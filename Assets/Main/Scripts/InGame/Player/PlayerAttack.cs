using UnityEngine;
using System.Collections.Generic;
using Main.Enemy;

namespace Main.InGame.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D attackColl;
        [SerializeField] private int damageAmount = 1;

        private readonly HashSet<int> damagedEnemyIds = new HashSet<int>();
        private readonly Collider2D[] overlapResults = new Collider2D[16];

        private void Awake()
        {
            attackColl = GetComponent<BoxCollider2D>();
            attackColl.isTrigger = true;
            attackColl.enabled = false;
        }

        public void PlayerAttackColliderEnable()
        {
            damagedEnemyIds.Clear();
            attackColl.enabled = true;

            var filter = new ContactFilter2D
            {
                useTriggers = true,
                useLayerMask = false,
                useDepth = false,
                useNormalAngle = false
            };

            int count = attackColl.Overlap(filter, overlapResults);
            for (int i = 0; i < count; i++)
            {
                TryDamage(overlapResults[i]);
            }
        }

        public void PlayerAttackColliderDisable()
        {
            attackColl.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryDamage(other);
        }

        private void TryDamage(Collider2D other)
        {
            if (other == null) return;
            if (!other.CompareTag("Enemy")) return;

            var enemyHealth = other.GetComponentInParent<EnemyHealth>();
            if (enemyHealth == null) return;
            if (enemyHealth.IsDead) return;

            int id = enemyHealth.GetInstanceID();
            if (!damagedEnemyIds.Add(id)) return;

            enemyHealth.TakeDamage(damageAmount);
        }
    }
}
