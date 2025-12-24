using UnityEngine;

namespace Main.InGame.Core
{
    public sealed class RecordingPlaybackSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RecordingSystem recordingSystem;
        [Tooltip("複製元になるプレイヤー（シーン上の実体）")]
        [SerializeField] private GameObject playerSource;

        [Header("Playback")]
        [SerializeField] private bool destroyGhostOnStop = true;

        private RecordingClip clip;
        private GameObject ghost;
        private RecordableEntity ghostRecordable;
        private Rigidbody2D ghostRb2d;
        private int entityIndex;
        private int frameIndex;
        private float accumulator;

        public bool IsPlaying => clip != null;

        private void Awake()
        {
            if (recordingSystem == null)
            {
                recordingSystem = FindAnyObjectByType<RecordingSystem>();
            }

            if (playerSource == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerSource = player;
            }
        }

        private void FixedUpdate()
        {
            if (clip == null) return;

            accumulator += Time.fixedDeltaTime;
            while (accumulator >= clip.sampleInterval)
            {
                accumulator -= clip.sampleInterval;
                ApplyFrameAndAdvance();

                if (clip == null) break; // StopPlayback() が呼ばれた場合
            }
        }

        public void PlayLastClip()
        {
            if (recordingSystem == null) return;
            Play(recordingSystem.LastClip);
        }

        public void Play(RecordingClip recordingClip)
        {
            if (recordingClip == null) return;
            if (playerSource == null) return;

            StopPlayback();

            clip = recordingClip;
            accumulator = 0f;
            frameIndex = 0;

            ghost = Instantiate(playerSource, playerSource.transform.position, playerSource.transform.rotation);
            ghost.name = playerSource.name + "_Ghost";

            // 入力や自動移動で上書きされないように無効化
            var controller = ghost.GetComponent<Main.Player.PlayerController>();
            if (controller != null) controller.enabled = false;
            var move = ghost.GetComponent<Main.Player.PlayerMove>();
            if (move != null) move.enabled = false;

            ghostRecordable = ghost.GetComponent<RecordableEntity>();
            if (ghostRecordable == null)
            {
                StopPlayback();
                return;
            }

            if (!clip.TryGetEntityIndex(ghostRecordable.EntityId, out entityIndex))
            {
                StopPlayback();
                return;
            }

            ghostRb2d = ghost.GetComponent<Rigidbody2D>();
            if (ghostRb2d != null)
            {
                // 物理で動かさず、再生位置を優先
                ghostRb2d.linearVelocity = Vector2.zero;
                ghostRb2d.angularVelocity = 0f;
                ghostRb2d.bodyType = RigidbodyType2D.Kinematic;
            }

            // 1フレーム目を即反映
            ApplyFrame();
        }

        public void StopPlayback()
        {
            clip = null;
            accumulator = 0f;
            frameIndex = 0;
            entityIndex = 0;

            ghostRecordable = null;
            ghostRb2d = null;

            if (ghost != null)
            {
                if (destroyGhostOnStop) Destroy(ghost);
                ghost = null;
            }
        }

        private void ApplyFrameAndAdvance()
        {
            if (clip == null) return;

            frameIndex++;
            if (frameIndex >= clip.frameCount)
            {
                StopPlayback();
                return;
            }

            ApplyFrame();
        }

        private void ApplyFrame()
        {
            if (clip == null || ghost == null) return;

            var pos = clip.GetPosition(frameIndex, entityIndex);
            var rot = clip.GetRotation(frameIndex, entityIndex);

            if (ghostRb2d != null)
            {
                ghostRb2d.MovePosition(new Vector2(pos.x, pos.y));
            }
            else
            {
                ghost.transform.position = pos;
            }

            // PlayerMove は y軸180度反転を使うので、2D回転ではなくTransform回転を適用
            ghost.transform.rotation = rot;

            // MovePosition はXYのみなのでZだけ補正
            var current = ghost.transform.position;
            current.z = pos.z;
            ghost.transform.position = current;
        }
    }
}
