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
                rotations.ToArray());
        }
    }
}
