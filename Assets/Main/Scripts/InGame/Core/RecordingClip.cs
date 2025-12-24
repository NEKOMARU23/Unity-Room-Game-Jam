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

        public int EntityCount => entityIds?.Length ?? 0;
        public float Duration => frameCount <= 0 ? 0f : (frameCount - 1) * sampleInterval;

        public RecordingClip(
            float sampleInterval,
            string[] entityIds,
            int frameCount,
            Vector3[] positions,
            Quaternion[] rotations)
        {
            if (sampleInterval <= 0f) throw new ArgumentOutOfRangeException(nameof(sampleInterval));
            this.sampleInterval = sampleInterval;
            this.entityIds = entityIds ?? Array.Empty<string>();
            this.frameCount = frameCount;
            this.positions = positions ?? Array.Empty<Vector3>();
            this.rotations = rotations ?? Array.Empty<Quaternion>();
        }

        public Vector3 GetPosition(int frameIndex, int entityIndex)
        {
            return positions[frameIndex * EntityCount + entityIndex];
        }

        public Quaternion GetRotation(int frameIndex, int entityIndex)
        {
            return rotations[frameIndex * EntityCount + entityIndex];
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
