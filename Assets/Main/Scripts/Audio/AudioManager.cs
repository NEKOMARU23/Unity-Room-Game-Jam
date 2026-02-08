using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Main.Scene;

namespace Main.Audio
{
    /// <summary>
    /// 全体の音響（BGM/SE）を管理する。
    /// 指定範囲再生（StartTime ～ Duration）によるフェードアウトカット機能を搭載。
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        protected override bool UseDontDestroyOnLoad => true;

        [Header("Audio Data")]
        [SerializeField] private AudioDataSO[] audioDataArray;
        [SerializeField] private int maxAudioSources = 10;

        [Header("Volume Settings")]
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float seVolume = 1f;
        
        private Dictionary<string, AudioData> audioDictionary;
        private List<AudioSource> audioSourcePool;
        private Queue<AudioSource> availableAudioSources;

        // BGMの区間制御用変数
        private AudioSource activeBGMSource;
        private float currentBGMStartTime;
        private float currentBGMDuration;

        private const string MASTER_VOLUME_KEY = "MasterVolume";
        private const string BGM_VOLUME_KEY = "BGMVolume";
        private const string SE_VOLUME_KEY = "SEVolume";

        public float MasterVolume => masterVolume;
        public float BGMVolume => bgmVolume;
        public float SEVolume => seVolume;

        protected override void Awake()
        {
            base.Awake();
            LoadVolumeSettings();
            InitializeAudioDictionary();
            SetupAudioSourcePool();
        }

        private void Update()
        {
            MonitorBGMLoopRange();
        }

        /// <summary>
        /// BGMの再生時間を監視し、Durationを超えたら開始位置へ戻す（フェードアウト回避）
        /// </summary>
        private void MonitorBGMLoopRange()
        {
            if (activeBGMSource == null || !activeBGMSource.isPlaying || currentBGMDuration <= 0f) return;

            // 再生時間が (開始地点 + 再生すべき長さ) を超えたらループ
            if (activeBGMSource.time >= currentBGMStartTime + currentBGMDuration)
            {
                activeBGMSource.time = currentBGMStartTime;
            }
        }

        private void LoadVolumeSettings()
        {
            masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
            bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
            seVolume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, 1f);
        }

        private void InitializeAudioDictionary()
        {
            audioDictionary = new Dictionary<string, AudioData>();
            if (audioDataArray == null) return;

            foreach (var so in audioDataArray)
            {
                if (so == null || so.AudioDataList == null) continue;
                foreach (var data in so.AudioDataList)
                {
                    if (data == null || string.IsNullOrEmpty(data.AudioName)) continue;
                    audioDictionary[data.AudioName] = data;
                }
            }
        }

        private void SetupAudioSourcePool()
        {
            audioSourcePool = new List<AudioSource>();
            availableAudioSources = new Queue<AudioSource>();

            for (int i = 0; i < maxAudioSources; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                audioSourcePool.Add(source);
                availableAudioSources.Enqueue(source);
            }
        }

        /// <summary>
        /// SEを再生します。
        /// </summary>
        public void Play(string audioName)
        {
            if (!audioDictionary.TryGetValue(audioName, out AudioData data)) return;
            if (data.AudioClip == null) return;

            AudioSource source = GetAvailableAudioSource();
            if (source == null) return;

            source.clip = data.AudioClip;
            source.volume = GetCalculatedVolume(data, false);
            source.loop = false;
            source.time = 0f;
            source.Play();

            StartCoroutine(ReturnAudioSourceWhenFinished(source));
        }

        /// <summary>
        /// BGMを再生します。
        /// </summary>
        /// <param name="bgmName">曲名</param>
        /// <param name="startTime">開始秒数</param>
        /// <param name="duration">再生する長さ（秒）。0以下の場合は最後まで。終了間際のフェードを消すのに使用。</param>
        public void PlayBGM(string bgmName, float startTime = 0f, float duration = 0f)
        {
            if (!audioDictionary.TryGetValue(bgmName, out AudioData data)) return;
            if (data.AudioClip == null) return;

            StopBGM();
            
            AudioSource source = GetAvailableAudioSource();
            if (source == null) return;

            source.clip = data.AudioClip;
            source.volume = GetCalculatedVolume(data, true);
            
            // Duration指定がある場合は自前Updateで制御するため、AudioSource側のloopはfalseにする
            source.loop = (duration <= 0f);
            
            source.time = Mathf.Clamp(startTime, 0f, data.AudioClip.length - 0.1f);
            source.Play();

            // 監視用変数をセット
            activeBGMSource = source;
            currentBGMStartTime = startTime;
            currentBGMDuration = duration;
        }

        public void StopBGM()
        {
            if (activeBGMSource != null)
            {
                activeBGMSource.Stop();
                activeBGMSource.loop = false;
                availableAudioSources.Enqueue(activeBGMSource);
                activeBGMSource = null;
            }
            currentBGMDuration = 0f;
        }

        private float GetCalculatedVolume(AudioData data, bool isBGM)
        {
            float categoryVolume = isBGM ? bgmVolume : seVolume;
            return data.VolumeMultiplier * categoryVolume * masterVolume;
        }

        private AudioSource GetAvailableAudioSource()
        {
            if (availableAudioSources.Count > 0) return availableAudioSources.Dequeue();
            foreach (var s in audioSourcePool) { if (!s.isPlaying) return s; }
            return null;
        }

        private IEnumerator ReturnAudioSourceWhenFinished(AudioSource source)
        {
            yield return new WaitWhile(() => source.isPlaying);
            availableAudioSources.Enqueue(source);
        }

        public void SetMasterVolume(float vol) { masterVolume = vol; UpdateAllVolumes(); }
        public void SetBGMVolume(float vol) { bgmVolume = vol; UpdateAllVolumes(); }
        public void SetSEVolume(float vol) { seVolume = vol; UpdateAllVolumes(); }

        private void UpdateAllVolumes()
        {
            foreach (var source in audioSourcePool)
            {
                if (!source.isPlaying || source.clip == null) continue;
                string audioName = GetAudioNameFromClip(source.clip);
                if (audioDictionary.TryGetValue(audioName, out AudioData data))
                {
                    bool isBGM = (source == activeBGMSource);
                    source.volume = GetCalculatedVolume(data, isBGM);
                }
            }
        }

        private string GetAudioNameFromClip(AudioClip clip)
        {
            foreach (var kvp in audioDictionary)
            {
                if (kvp.Value.AudioClip == clip) return kvp.Key;
            }
            return string.Empty;
        }
    }
}