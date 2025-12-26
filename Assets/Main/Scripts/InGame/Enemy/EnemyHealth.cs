using UnityEngine;
using Main.Damage;

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
            if (isDead) return; // 二重実行防止
            isDead = true;

            // 1. 見た目を変更
            if (deadSprite != null) spriteRenderer.sprite = deadSprite;

            // 2. 自律移動スクリプトを止める
            if (TryGetComponent<EnemyMove>(out EnemyMove move)) move.enabled = false;

            // 3. 物理挙動：プレイヤーが押して動かせる設定に変更
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.freezeRotation = true;
                rb.WakeUp(); // 物理演算を強制再開
                rb.linearDamping = 0.5f;
                rb.mass = 0.8f;
            }

            // 4. 判定の整理：本体を実体化し、攻撃判定だけを消す
            Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
            foreach (var col in allColliders)
            {
                // そのコライダーのオブジェクトに DamageSource が付いている場合
                if (col.TryGetComponent<DamageSource>(out var ds))
                {
                    ds.enabled = false; // スクリプトをOFF
                    col.enabled = false; // ★攻撃用コライダーだけをピンポイントで消す
                }
                else
                {
                    // 本体（地面用）のコライダーは Trigger を解除して「動かせる壁」にする
                    col.isTrigger = false;
                }
            }

            // 5. レイヤー変更（プレイヤーの足場として認識されるように）
            int groundLayer = LayerMask.NameToLayer(groundLayerName);
            if (groundLayer != -1) gameObject.layer = groundLayer;
        }
    }
}