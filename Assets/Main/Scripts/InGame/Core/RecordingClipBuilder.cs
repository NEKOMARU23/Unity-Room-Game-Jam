using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.InGame.Core
{
    internal sealed class RecordingClipBuilder
    {
        private readonly float sampleInterval;
        private readonly string[] entityIds;
        private readonly int entityCount;

        private readonly List<Vector3> positions = new();
        private readonly List<Quaternion> rotations = new();
        private readonly List<byte> activeFlags = new();
        private readonly List<int> animatorStateHashes = new();
        private readonly List<float> animatorNormalizedTimes = new();

        private readonly List<float> animatorSpeed = new();
        private readonly List<byte> animatorJumpFlags = new();
        private readonly List<byte> animatorAttackFlags = new();

        private readonly List<byte> spriteFlipXFlags = new();

        private readonly List<byte> enemyDeadFlags = new();

        public int FrameCount { get; private set; }

        public RecordingClipBuilder(float sampleInterval, string[] entityIds)
        {
            if (sampleInterval <= 0f) throw new ArgumentOutOfRangeException(nameof(sampleInterval));
            this.sampleInterval = sampleInterval;
            this.entityIds = entityIds ?? Array.Empty<string>();
            entityCount = this.entityIds.Length;
        }

        public void AddFrame(IReadOnlyList<RecordableEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (entities.Count != entityCount)
            {
                throw new InvalidOperationException("Entity count changed during recording. Keep the set of RecordableEntity stable during a clip.");
            }

            for (int i = 0; i < entityCount; i++)
            {
                entities[i].Capture(out var pos, out var rot);
                positions.Add(pos);
                rotations.Add(rot);

                // gameplay state (alive/dead etc.)
                activeFlags.Add(entities[i].gameObject.activeSelf ? (byte)1 : (byte)0);

                // animation state snapshot (layer 0)
                var animator = entities[i].GetComponent<Animator>();
                if (animator != null)
                {
                    var info = animator.GetCurrentAnimatorStateInfo(0);
                    animatorStateHashes.Add(info.fullPathHash);
                    // normalize to 0..1 so it can be safely used with Animator.Play
                    animatorNormalizedTimes.Add(Mathf.Repeat(info.normalizedTime, 1f));

                    // parameters that actually drive transitions
                    animatorSpeed.Add(animator.GetFloat("Speed"));
                    animatorJumpFlags.Add(animator.GetBool("Jump") ? (byte)1 : (byte)0);
                    animatorAttackFlags.Add(info.IsName("Attack") ? (byte)1 : (byte)0);
                }
                else
                {
                    animatorStateHashes.Add(0);
                    animatorNormalizedTimes.Add(0f);

                    animatorSpeed.Add(0f);
                    animatorJumpFlags.Add(0);
                    animatorAttackFlags.Add(0);
                }

                // sprite facing (flipX)
                var spriteRenderer = entities[i].GetComponentInChildren<SpriteRenderer>(true);
                spriteFlipXFlags.Add(spriteRenderer != null && spriteRenderer.flipX ? (byte)1 : (byte)0);

                // enemy death (sprite swap etc.)
                var enemyHealth = entities[i].GetComponent<Main.Enemy.EnemyHealth>();
                enemyDeadFlags.Add(enemyHealth != null && enemyHealth.IsDead ? (byte)1 : (byte)0);
            }

            FrameCount++;
        }

        public RecordingClip Build()
        {
            return new RecordingClip(
                sampleInterval,
                entityIds,
                FrameCount,
                positions.ToArray(),
                rotations.ToArray(),
                activeFlags.ToArray(),
                animatorStateHashes.ToArray(),
                animatorNormalizedTimes.ToArray(),
                animatorSpeed.ToArray(),
                animatorJumpFlags.ToArray(),
                animatorAttackFlags.ToArray(),
                spriteFlipXFlags.ToArray(),
                enemyDeadFlags.ToArray());
        }
    }
}
