using UnityEngine;

namespace Main.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private int maxHealth = 1;
        [SerializeField] private Sprite deadSprite;         // 倒れた時の画像
        [SerializeField] private string groundLayerName = "Ground"; // 接地判定用のレイヤー名

        private int currentHealth;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb;
        private bool isDead = false;

        void Awake()
        {
            currentHealth = maxHealth;
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // すでに死んでいる、または攻撃判定以外なら無視
            if (isDead || !other.CompareTag("PlayerAttack")) return;

            TakeDamage(1);
        }

        public void TakeDamage(int damage)
        {
            if (isDead) return;

            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;

            // 1. 見た目を倒れた画像に変更
            if (deadSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = deadSprite;
            }

            // 2. 移動スクリプトを停止（EnemyMoveがついている場合）
            if (TryGetComponent<EnemyMove>(out EnemyMove move))
            {
                move.enabled = false;
            }

            // 3. 物理的に固定（押されても動かない「静的オブジェクト」化）
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Static;
            }

            // 4. レイヤーをGroundに変更（接地判定の対象にする）
            int groundLayer = LayerMask.NameToLayer(groundLayerName);
            if (groundLayer != -1)
            {
                gameObject.layer = groundLayer;
            }
            
            // 5. 描画順を少し下げて、プレイヤーの背後に回す
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder -= 1;
            }
        }
    }
}