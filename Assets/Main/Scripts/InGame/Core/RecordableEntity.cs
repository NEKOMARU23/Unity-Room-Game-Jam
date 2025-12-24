using System;
using UnityEngine;

namespace Main.InGame.Core
{
    [DisallowMultipleComponent]
    public sealed class RecordableEntity : MonoBehaviour
    {
        [SerializeField] private string entityId;

        private Rigidbody2D rb2d;

        public string EntityId => entityId;

        private void OnValidate()
        {
            // editor上でIDを確定させて、録画・再生・複製で一致するようにする
            if (string.IsNullOrWhiteSpace(entityId))
            {
                entityId = Guid.NewGuid().ToString("N");
            }
        }

        private void Awake()
        {
            rb2d = GetComponent<Rigidbody2D>();

            if (string.IsNullOrWhiteSpace(entityId))
            {
                entityId = Guid.NewGuid().ToString("N");
            }
        }

        public void ForceSetEntityId(string newEntityId)
        {
            if (string.IsNullOrWhiteSpace(newEntityId)) return;
            entityId = newEntityId;
        }

        public void Capture(out Vector3 position, out Quaternion rotation)
        {
            if (rb2d != null)
            {
                var rbPos = rb2d.position;
                position = new Vector3(rbPos.x, rbPos.y, transform.position.z);
            }
            else
            {
                position = transform.position;
            }

            rotation = transform.rotation;
        }
    }
}
