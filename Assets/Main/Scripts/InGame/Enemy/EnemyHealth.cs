using UnityEngine;

namespace Main.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private int maxHealth = 1;
        private int currentHealth;

        void Awake()
        {
            currentHealth = maxHealth;
        }

        // プレイヤーの攻撃判定（Trigger）が触れたときに呼ばれる
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 当たった相手のタグを確認
            // 先ほど作った AttackRange オブジェクトに "Player" タグが付いているか、
            // または特定の名前で判定します
            if (other.CompareTag("PlayerAttack")) 
            {
                TakeDamage(1);
            }
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}