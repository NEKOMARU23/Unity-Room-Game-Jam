using System;
using UnityEngine;

namespace Main.InGame.Core
{
    /// <summary>
    /// Runtime recording clip. Not a ScriptableObject; stored in-memory.
    /// </summary>
    public sealed class RecordingClip
    {
        public readonly float sampleInterval;
        public readonly string[] entityIds;
        public readonly int frameCount;

        // Flattened arrays: index = frameIndex * entityCount + entityIndex
        public readonly Vector3[] positions;
        public readonly Quaternion[] rotations;

        // Optional state tracks (same flatten indexing as positions/rotations)
        public readonly byte[] activeFlags;
        public readonly int[] animatorStateHashes;
        public readonly float[] animatorNormalizedTimes;

        // Animator parameters (layer 0 driven by scripts)
        public readonly float[] animatorSpeed;
        public readonly byte[] animatorJumpFlags;
        public readonly byte[] animatorAttackFlags;

        // Sprite facing
        public readonly byte[] spriteFlipXFlags;

        // Enemy state
        public readonly byte[] enemyDeadFlags;

        public int EntityCount => entityIds?.Length ?? 0;
        public float Duration => frameCount <= 0 ? 0f : (frameCount - 1) * sampleInterval;

        public RecordingClip(
            float sampleInterval,
            string[] entityIds,
            int frameCount,
            Vector3[] positions,
            Quaternion[] rotations,
            byte[] activeFlags,
            int[] animatorStateHashes,
            float[] animatorNormalizedTimes,
            float[] animatorSpeed,
            byte[] animatorJumpFlags,
            byte[] animatorAttackFlags,
            byte[] spriteFlipXFlags,
            byte[] enemyDeadFlags)
        {
            if (sampleInterval <= 0f) throw new ArgumentOutOfRangeException(nameof(sampleInterval));
            this.sampleInterval = sampleInterval;
            this.entityIds = entityIds ?? Array.Empty<string>();
            this.frameCount = frameCount;
            this.positions = positions ?? Array.Empty<Vector3>();
            this.rotations = rotations ?? Array.Empty<Quaternion>();

            this.activeFlags = activeFlags ?? Array.Empty<byte>();
            this.animatorStateHashes = animatorStateHashes ?? Array.Empty<int>();
            this.animatorNormalizedTimes = animatorNormalizedTimes ?? Array.Empty<float>();

            this.animatorSpeed = animatorSpeed ?? Array.Empty<float>();
            this.animatorJumpFlags = animatorJumpFlags ?? Array.Empty<byte>();
            this.animatorAttackFlags = animatorAttackFlags ?? Array.Empty<byte>();

            this.spriteFlipXFlags = spriteFlipXFlags ?? Array.Empty<byte>();

            this.enemyDeadFlags = enemyDeadFlags ?? Array.Empty<byte>();
        }

        public Vector3 GetPosition(int frameIndex, int entityIndex)
        {
            return positions[frameIndex * EntityCount + entityIndex];
        }

        public Quaternion GetRotation(int frameIndex, int entityIndex)
        {
            return rotations[frameIndex * EntityCount + entityIndex];
        }

        public bool GetActive(int frameIndex, int entityIndex)
        {
            if (activeFlags == null || activeFlags.Length == 0) return true;
            return activeFlags[frameIndex * EntityCount + entityIndex] != 0;
        }

        public bool TryGetAnimatorState(int frameIndex, int entityIndex, out int stateHash, out float normalizedTime)
        {
            stateHash = 0;
            normalizedTime = 0f;

            if (animatorStateHashes == null || animatorNormalizedTimes == null) return false;
            if (animatorStateHashes.Length == 0 || animatorNormalizedTimes.Length == 0) return false;

            int idx = frameIndex * EntityCount + entityIndex;
            if (idx < 0 || idx >= animatorStateHashes.Length || idx >= animatorNormalizedTimes.Length) return false;

            stateHash = animatorStateHashes[idx];
            normalizedTime = animatorNormalizedTimes[idx];
            return stateHash != 0;
        }

        public float GetAnimatorSpeed(int frameIndex, int entityIndex)
        {
            if (animatorSpeed == null || animatorSpeed.Length == 0) return 0f;
            return animatorSpeed[frameIndex * EntityCount + entityIndex];
        }

        public bool GetAnimatorJump(int frameIndex, int entityIndex)
        {
            if (animatorJumpFlags == null || animatorJumpFlags.Length == 0) return false;
            return animatorJumpFlags[frameIndex * EntityCount + entityIndex] != 0;
        }

        public bool GetAnimatorAttack(int frameIndex, int entityIndex)
        {
            if (animatorAttackFlags == null || animatorAttackFlags.Length == 0) return false;
            return animatorAttackFlags[frameIndex * EntityCount + entityIndex] != 0;
        }

        public bool GetSpriteFlipX(int frameIndex, int entityIndex)
        {
            if (spriteFlipXFlags == null || spriteFlipXFlags.Length == 0) return false;
            return spriteFlipXFlags[frameIndex * EntityCount + entityIndex] != 0;
        }

        public bool GetEnemyDead(int frameIndex, int entityIndex)
        {
            if (enemyDeadFlags == null || enemyDeadFlags.Length == 0) return false;
            return enemyDeadFlags[frameIndex * EntityCount + entityIndex] != 0;
        }

        public bool TryGetEntityIndex(string entityId, out int index)
        {
            index = -1;
            if (string.IsNullOrEmpty(entityId)) return false;
            if (entityIds == null) return false;

            for (int i = 0; i < entityIds.Length; i++)
            {
                if (entityIds[i] == entityId)
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }
    }
}
