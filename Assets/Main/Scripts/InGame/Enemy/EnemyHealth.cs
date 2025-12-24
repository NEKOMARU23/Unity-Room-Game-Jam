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

            if (deadSprite != null) spriteRenderer.sprite = deadSprite;

            if (TryGetComponent<EnemyMove>(out EnemyMove move)) move.enabled = false;

            // --- 物理挙動の強制リセット ---
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic; // 動ける状態にする
                rb.freezeRotation = true;              // 転がらないようにする

                // 重要：スリープ状態（計算停止）を解除して、物理演算を強制開始させる
                rb.WakeUp();

                // 押しやすくするための設定
                rb.linearDamping = 0.5f;   // 空気抵抗（以前のDrag）を低く
                rb.mass = 0.8f;            // プレイヤーより少し軽くすると押しやすい
            }

            // コライダーを実体化（すり抜け防止）
            if (TryGetComponent<Collider2D>(out Collider2D col))
            {
                col.isTrigger = false;

                // もし滑りが悪いなら、プログラムから摩擦0のマテリアルを適用する
                // PhysicsMaterial2D frictionZero = new PhysicsMaterial2D { friction = 0, bounciness = 0 };
                // col.sharedMaterial = frictionZero;
            }

            // レイヤー変更
            int groundLayer = LayerMask.NameToLayer(groundLayerName);
            if (groundLayer != -1) gameObject.layer = groundLayer;
        }
    }
}