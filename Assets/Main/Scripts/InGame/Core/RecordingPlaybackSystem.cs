using UnityEngine;

namespace Main.InGame.Core
{
    public sealed class RecordingPlaybackSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RecordingSystem recordingSystem;

        [Header("Playback")]
        [Tooltip("再生開始までの待機秒数")]
        [SerializeField] private float playbackStartDelaySeconds = 1.1f;
        [SerializeField] private bool destroyGhostOnStop = true;

        private sealed class PlaybackTarget
        {
            public int entityIndex;
            public Transform transform;
            public Rigidbody2D rb2d;
            public Animator animator;
            public SpriteRenderer spriteRenderer;
            public Main.Enemy.EnemyHealth enemyHealth;
            public bool wasAttacking;

            public GameObject ownedInstance;
            public Behaviour[] disabledBehaviours;
            public RigidbodyType2D? originalBodyType;
            public AnimatorUpdateMode? originalAnimatorUpdateMode;
            public float originalAnimatorSpeed;
            public bool originalAnimatePhysics;

            public bool originalFlipX;
            public bool hasOriginalFlipX;
        }

        private readonly System.Collections.Generic.List<PlaybackTarget> targets = new();

        private RecordingClip clip;
        private int frameIndex;
        private float accumulator;

        // Play() 直後に 1フレーム目を反映し、その状態で一定時間ホールドしてからフレーム進行を開始する
        private float startHoldRemainingSeconds;

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

            // 1フレーム目の状態でホールド
            if (startHoldRemainingSeconds > 0f)
            {
                startHoldRemainingSeconds -= Time.fixedDeltaTime;
                return;
            }

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

            StartPlaybackNow(recordingClip);
            startHoldRemainingSeconds = Mathf.Max(0f, playbackStartDelaySeconds);
        }

        public void StopPlayback()
        {
            startHoldRemainingSeconds = 0f;
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

                if (t.animator != null)
                {
                    if (t.originalAnimatorUpdateMode.HasValue) t.animator.updateMode = t.originalAnimatorUpdateMode.Value;
                    t.animator.speed = t.originalAnimatorSpeed;
                    t.animator.animatePhysics = t.originalAnimatePhysics;
                }

                if (t.spriteRenderer != null && t.hasOriginalFlipX)
                {
                    t.spriteRenderer.flipX = t.originalFlipX;
                }

                if (t.ownedInstance != null && destroyGhostOnStop)
                {
                    Destroy(t.ownedInstance);
                }
            }

            targets.Clear();
        }

        private void StartPlaybackNow(RecordingClip recordingClip)
        {
            if (recordingClip == null) return;

            clip = recordingClip;
            accumulator = 0f;
            frameIndex = 0;

            BuildTargetsFromScene();
            if (targets.Count == 0)
            {
                clip = null;
                accumulator = 0f;
                frameIndex = 0;
                targets.Clear();
                return;
            }

            // 1フレーム目を即反映
            ApplyFrame();
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

                // alive/dead (active)
                var go = t.transform.gameObject;
                bool shouldBeActive = clip.GetActive(frameIndex, t.entityIndex);
                if (go.activeSelf != shouldBeActive)
                {
                    go.SetActive(shouldBeActive);
                }

                // enemy death appearance/state (sprite swap etc.)
                if (t.enemyHealth != null)
                {
                    bool shouldBeDead = clip.GetEnemyDead(frameIndex, t.entityIndex);
                    t.enemyHealth.ApplyRecordedDeathState(shouldBeDead);
                }

                // facing
                if (t.spriteRenderer != null)
                {
                    t.spriteRenderer.flipX = clip.GetSpriteFlipX(frameIndex, t.entityIndex);
                }

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

                // animation
                if (t.animator != null && shouldBeActive)
                {
                    // parameter-driven playback to avoid state-machine fighting
                    t.animator.SetFloat("Speed", clip.GetAnimatorSpeed(frameIndex, t.entityIndex));
                    t.animator.SetBool("Jump", clip.GetAnimatorJump(frameIndex, t.entityIndex));

                    bool attacking = clip.GetAnimatorAttack(frameIndex, t.entityIndex);
                    if (attacking && !t.wasAttacking)
                    {
                        t.animator.SetTrigger("Attack");
                    }
                    t.wasAttacking = attacking;
                }
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
                                srcGo.GetComponent<Main.InGame.Player.PlayerMove>() != null ||
                                srcGo.GetComponent<Main.InGame.Player.PlayerController>() != null;

                if (isPlayer)
                {
                    // プレイヤーは複製してゴーストを動かす
                    var ghost = Instantiate(srcGo, srcGo.transform.position, srcGo.transform.rotation);
                    ghost.name = srcGo.name + "_Ghost";
                    // Playerタグのままだと、他の処理（FindWithTag/Trigger判定など）がゴーストをプレイヤー扱いしてしまう
                    ghost.tag = "Untagged";
                    ghost.gameObject.layer = LayerMask.NameToLayer("Player_Recorded");

                    // ゴーストと現プレイヤーがぶつかって押し合わないように、Collider同士の衝突だけ無効化する
                    IgnoreCollisionsBetween(ghost, srcGo);

                    var ghostRecordable = ghost.GetComponent<RecordableEntity>();
                    if (ghostRecordable != null)
                    {
                        ghostRecordable.ForceSetEntityId(id);
                    }

                    var disabled = DisableMovementBehaviours(ghost);
                    var rb2d = ghost.GetComponent<Rigidbody2D>();
                    var animator = ghost.GetComponent<Animator>();
                    var spriteRenderer = ghost.GetComponentInChildren<SpriteRenderer>(true);
                    var enemyHealth = ghost.GetComponent<Main.Enemy.EnemyHealth>();
                    RigidbodyType2D? originalBodyType = null;
                    if (rb2d != null)
                    {
                        originalBodyType = rb2d.bodyType;
                        rb2d.linearVelocity = Vector2.zero;
                        rb2d.angularVelocity = 0f;
                        rb2d.bodyType = RigidbodyType2D.Kinematic;
                    }

                    AnimatorUpdateMode? originalUpdateMode = null;
                    float originalAnimSpeed = 1f;
                    bool originalAnimatePhysics = false;
                    if (animator != null)
                    {
                        originalUpdateMode = animator.updateMode;
                        originalAnimSpeed = animator.speed;
                        originalAnimatePhysics = animator.animatePhysics;
                        animator.updateMode = AnimatorUpdateMode.Fixed;
                        animator.animatePhysics = true;
                    }

                    targets.Add(new PlaybackTarget
                    {
                        entityIndex = entityIndex,
                        transform = ghost.transform,
                        rb2d = rb2d,
                        animator = animator,
                        spriteRenderer = spriteRenderer,
                        enemyHealth = enemyHealth,
                        ownedInstance = ghost,
                        disabledBehaviours = disabled,
                        originalBodyType = originalBodyType,
                        originalAnimatorUpdateMode = originalUpdateMode,
                        originalAnimatorSpeed = originalAnimSpeed,
                        originalAnimatePhysics = originalAnimatePhysics,
                        originalFlipX = spriteRenderer != null && spriteRenderer.flipX,
                        hasOriginalFlipX = spriteRenderer != null,
                    });
                }
                else
                {
                    // 敵（その他）は現物を動かす。上書き防止で移動スクリプトを止める
                    var disabled = DisableMovementBehaviours(srcGo);
                    var rb2d = srcGo.GetComponent<Rigidbody2D>();
                    var animator = srcGo.GetComponent<Animator>();
                    var spriteRenderer = srcGo.GetComponentInChildren<SpriteRenderer>(true);
                    var enemyHealth = srcGo.GetComponent<Main.Enemy.EnemyHealth>();
                    RigidbodyType2D? originalBodyType = null;
                    if (rb2d != null)
                    {
                        originalBodyType = rb2d.bodyType;
                        rb2d.linearVelocity = Vector2.zero;
                        rb2d.angularVelocity = 0f;
                        rb2d.bodyType = RigidbodyType2D.Kinematic;
                    }

                    AnimatorUpdateMode? originalUpdateMode = null;
                    float originalAnimSpeed = 1f;
                    bool originalAnimatePhysics = false;
                    if (animator != null)
                    {
                        originalUpdateMode = animator.updateMode;
                        originalAnimSpeed = animator.speed;
                        originalAnimatePhysics = animator.animatePhysics;
                        animator.updateMode = AnimatorUpdateMode.Fixed;
                        animator.animatePhysics = true;
                    }

                    targets.Add(new PlaybackTarget
                    {
                        entityIndex = entityIndex,
                        transform = srcGo.transform,
                        rb2d = rb2d,
                        animator = animator,
                        spriteRenderer = spriteRenderer,
                        enemyHealth = enemyHealth,
                        ownedInstance = null,
                        disabledBehaviours = disabled,
                        originalBodyType = originalBodyType,
                        originalAnimatorUpdateMode = originalUpdateMode,
                        originalAnimatorSpeed = originalAnimSpeed,
                        originalAnimatePhysics = originalAnimatePhysics,
                        originalFlipX = spriteRenderer != null && spriteRenderer.flipX,
                        hasOriginalFlipX = spriteRenderer != null,
                    });
                }
            }
        }

        private static Behaviour[] DisableMovementBehaviours(GameObject go)
        {
            var list = new System.Collections.Generic.List<Behaviour>(4);

            var playerController = go.GetComponent<Main.InGame.Player.PlayerController>();
            if (playerController != null && playerController.enabled)
            {
                playerController.enabled = false;
                list.Add(playerController);
            }

            var playerMove = go.GetComponent<Main.InGame.Player.PlayerMove>();
            if (playerMove != null && playerMove.enabled)
            {
                playerMove.enabled = false;
                list.Add(playerMove);
            }

            // 再生中はAnimatorを直接駆動するので、PlayerAnimationの自動更新は止める
            var playerAnim = go.GetComponent<Main.InGame.Player.PlayerAnimation>();
            if (playerAnim != null && playerAnim.enabled)
            {
                playerAnim.enabled = false;
                list.Add(playerAnim);
            }

            var enemyMove = go.GetComponent<Main.Enemy.EnemyMove>();
            if (enemyMove != null && enemyMove.enabled)
            {
                enemyMove.enabled = false;
                list.Add(enemyMove);
            }

            return list.Count == 0 ? null : list.ToArray();
        }

        private static void IgnoreCollisionsBetween(GameObject a, GameObject b)
        {
            if (a == null || b == null) return;

            var aCols = a.GetComponentsInChildren<Collider2D>(true);
            var bCols = b.GetComponentsInChildren<Collider2D>(true);
            if (aCols == null || bCols == null) return;

            for (int i = 0; i < aCols.Length; i++)
            {
                var ac = aCols[i];
                if (ac == null) continue;

                for (int j = 0; j < bCols.Length; j++)
                {
                    var bc = bCols[j];
                    if (bc == null) continue;

                    Physics2D.IgnoreCollision(ac, bc, true);
                }
            }
        }
    }
}
