using UnityEngine;

namespace Main.InGame.Core
{
    public class RecordingSystem : MonoBehaviour
    {

        [Header("Recording")]
        [Tooltip("FixedUpdateごとにサンプルします。移動がRigidbody2Dで更新されているので、この方がズレにくいです。")]
        [SerializeField] private bool sampleInFixedUpdate = true;

        private readonly System.Collections.Generic.List<RecordableEntity> recordables = new();

        private RecordingClipBuilder builder;
        public RecordingClip LastClip { get; private set; }

        public bool IsRecording => builder != null;

        private void FixedUpdate()
        {
            if (!sampleInFixedUpdate) return;
            if (!IsRecording) return;

            builder.AddFrame(recordables);
        }

        public void StartRecording()
        {
            if (IsRecording) return;

            CollectRecordables();

            var ids = new string[recordables.Count];
            for (int i = 0; i < recordables.Count; i++)
            {
                ids[i] = recordables[i].EntityId;
            }

            builder = new RecordingClipBuilder(Time.fixedDeltaTime, ids);
        }

        public void StopRecording()
        {
            if (!IsRecording) return;

            LastClip = builder.Build();
            builder = null;
        }

        /// <summary>
        /// 録画状態と直近クリップを破棄します（再生をキャンセルして色付きに戻す用途など）。
        /// </summary>
        public void ResetRecording()
        {
            builder = null;
            LastClip = null;
        }

        private void CollectRecordables()
        {
            recordables.Clear();

            var found = FindObjectsByType<RecordableEntity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (found == null || found.Length == 0) return;

            for (int i = 0; i < found.Length; i++)
            {
                // 無効オブジェクトは除外（FindObjectsOfTypeの挙動差異対策）
                if (!found[i].isActiveAndEnabled) continue;
                recordables.Add(found[i]);
            }
        }
    }
}
