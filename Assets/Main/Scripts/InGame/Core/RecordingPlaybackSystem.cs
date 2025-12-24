using UnityEngine;

namespace Main.InGame.Core
{
    public sealed class RecordingPlaybackSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RecordingSystem recordingSystem;

        [Header("Playback")]
        [SerializeField] private bool destroyGhostOnStop = true;

        private sealed class PlaybackTarget
        {
            public int entityIndex;
            public Transform transform;
            public Rigidbody2D rb2d;

            public GameObject ownedInstance;
            public Behaviour[] disabledBehaviours;
            public RigidbodyType2D? originalBodyType;
        }

        private readonly System.Collections.Generic.List<PlaybackTarget> targets = new();

        private RecordingClip clip;
        private int frameIndex;
        private float accumulator;

        public bool IsPlaying => clip != null;

        private void Awake()
        {
            if (recordingSystem == null)
            {
                recordingSystem = FindAnyObjectByType<RecordingSystem>();
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

            StopPlayback();

            clip = recordingClip;
            accumulator = 0f;
            frameIndex = 0;

            BuildTargetsFromScene();
            if (targets.Count == 0)
            {
                StopPlayback();
                return;
            }

            // 1フレーム目を即反映
            ApplyFrame();
        }

        public void StopPlayback()
        {
            clip = null;
            accumulator = 0f;
            frameIndex = 0;

            for (int i = 0; i < targets.Count; i++)
            {
                var t = targets[i];

                if (t.rb2d != null && t.originalBodyType.HasValue)
                {
                    t.rb2d.bodyType = t.originalBodyType.Value;
                }

                if (t.disabledBehaviours != null)
                {
                    for (int j = 0; j < t.disabledBehaviours.Length; j++)
                    {
                        if (t.disabledBehaviours[j] != null) t.disabledBehaviours[j].enabled = true;
                    }
                }

                if (t.ownedInstance != null && destroyGhostOnStop)
                {
                    Destroy(t.ownedInstance);
                }
            }

            targets.Clear();
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
            if (clip == null) return;

            for (int i = 0; i < targets.Count; i++)
            {
                var t = targets[i];
                if (t == null || t.transform == null) continue;

                var pos = clip.GetPosition(frameIndex, t.entityIndex);
                var rot = clip.GetRotation(frameIndex, t.entityIndex);

                if (t.rb2d != null)
                {
                    t.rb2d.MovePosition(new Vector2(pos.x, pos.y));
                }
                else
                {
                    t.transform.position = pos;
                }

                // PlayerMove/EnemyMove は y軸180度反転を使うので、Transform回転を適用
                t.transform.rotation = rot;

                // MovePosition はXYのみなのでZだけ補正
                var current = t.transform.position;
                current.z = pos.z;
                t.transform.position = current;
            }
        }

        private void BuildTargetsFromScene()
        {
            targets.Clear();

            var found = FindObjectsByType<RecordableEntity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (found == null || found.Length == 0) return;

            var map = new System.Collections.Generic.Dictionary<string, RecordableEntity>(found.Length);
            for (int i = 0; i < found.Length; i++)
            {
                var r = found[i];
                if (r == null || !r.isActiveAndEnabled) continue;
                if (string.IsNullOrEmpty(r.EntityId)) continue;
                if (!map.ContainsKey(r.EntityId)) map.Add(r.EntityId, r);
            }

            for (int entityIndex = 0; entityIndex < clip.EntityCount; entityIndex++)
            {
                var id = clip.entityIds[entityIndex];
                if (string.IsNullOrEmpty(id)) continue;
                if (!map.TryGetValue(id, out var recordable) || recordable == null) continue;

                var srcGo = recordable.gameObject;
                if (srcGo == null) continue;

                bool isPlayer = srcGo.CompareTag("Player") ||
                                srcGo.GetComponent<Main.Player.PlayerMove>() != null ||
                                srcGo.GetComponent<Main.Player.PlayerController>() != null;

                if (isPlayer)
                {
                    // プレイヤーは複製してゴーストを動かす
                    var ghost = Instantiate(srcGo, srcGo.transform.position, srcGo.transform.rotation);
                    ghost.name = srcGo.name + "_Ghost";
                    ghost.gameObject.layer = LayerMask.NameToLayer("Player_Recorded");

                    var ghostRecordable = ghost.GetComponent<RecordableEntity>();
                    if (ghostRecordable != null)
                    {
                        ghostRecordable.ForceSetEntityId(id);
                    }

                    var disabled = DisableMovementBehaviours(ghost);
                    var rb2d = ghost.GetComponent<Rigidbody2D>();
                    RigidbodyType2D? originalBodyType = null;
                    if (rb2d != null)
                    {
                        originalBodyType = rb2d.bodyType;
                        rb2d.linearVelocity = Vector2.zero;
                        rb2d.angularVelocity = 0f;
                        rb2d.bodyType = RigidbodyType2D.Kinematic;
                    }

                    targets.Add(new PlaybackTarget
                    {
                        entityIndex = entityIndex,
                        transform = ghost.transform,
                        rb2d = rb2d,
                        ownedInstance = ghost,
                        disabledBehaviours = disabled,
                        originalBodyType = originalBodyType,
                    });
                }
                else
                {
                    // 敵（その他）は現物を動かす。上書き防止で移動スクリプトを止める
                    var disabled = DisableMovementBehaviours(srcGo);
                    var rb2d = srcGo.GetComponent<Rigidbody2D>();
                    RigidbodyType2D? originalBodyType = null;
                    if (rb2d != null)
                    {
                        originalBodyType = rb2d.bodyType;
                        rb2d.linearVelocity = Vector2.zero;
                        rb2d.angularVelocity = 0f;
                        rb2d.bodyType = RigidbodyType2D.Kinematic;
                    }

                    targets.Add(new PlaybackTarget
                    {
                        entityIndex = entityIndex,
                        transform = srcGo.transform,
                        rb2d = rb2d,
                        ownedInstance = null,
                        disabledBehaviours = disabled,
                        originalBodyType = originalBodyType,
                    });
                }
            }
        }

        private static Behaviour[] DisableMovementBehaviours(GameObject go)
        {
            var list = new System.Collections.Generic.List<Behaviour>(4);

            var playerController = go.GetComponent<Main.Player.PlayerController>();
            if (playerController != null && playerController.enabled)
            {
                playerController.enabled = false;
                list.Add(playerController);
            }

            var playerMove = go.GetComponent<Main.Player.PlayerMove>();
            if (playerMove != null && playerMove.enabled)
            {
                playerMove.enabled = false;
                list.Add(playerMove);
            }

            var enemyMove = go.GetComponent<Main.Enemy.EnemyMove>();
            if (enemyMove != null && enemyMove.enabled)
            {
                enemyMove.enabled = false;
                list.Add(enemyMove);
            }

            return list.Count == 0 ? null : list.ToArray();
        }
    }
}
