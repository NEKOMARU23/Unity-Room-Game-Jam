using UnityEngine;
using Main.Damage;

namespace Main.Enemy
{
    /// <summary>
    /// エネミーの体力を管理し、死亡時の挙動や状態の復元を制御するクラス
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private int maxHealth = 1;
        [SerializeField] private Sprite deadSprite;
        [SerializeField] private string groundLayerName = "Ground";

        private const string PLAYER_ATTACK_TAG = "PlayerAttack";
        private const float DEAD_LINEAR_DAMPING = 0.5f;
        private const float DEAD_MASS = 0.8f;

        private int currentHealth;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb;
        private EnemyMove enemyMove;
        private bool isDead = false;

        private Sprite originalSprite;
        private int originalLayer;
        private int originalSortingOrder;
        private RigidbodyType2D originalBodyType;
        private bool originalEnemyMoveEnabled;
        private bool hasOriginalSnapshot;
        private int groundLayerHash;

        /// <summary>
        /// エネミーが死亡状態かどうかを取得する
        /// </summary>
        public bool IsDead => isDead;

        /// <summary>
        /// コンポーネントのキャッシュと初期値の設定を行う
        /// </summary>
        private void Awake()
        {
            currentHealth = maxHealth;
            
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            TryGetComponent(out enemyMove);

            groundLayerHash = LayerMask.NameToLayer(groundLayerName);

            CaptureOriginalSnapshot();
        }

        /// <summary>
        /// 初期状態のパラメータを保存する
        /// </summary>
        private void CaptureOriginalSnapshot()
        {
            if (hasOriginalSnapshot) return;

            originalLayer = gameObject.layer;

            if (spriteRenderer != null)
            {
                originalSprite = spriteRenderer.sprite;
                originalSortingOrder = spriteRenderer.sortingOrder;
            }
            else
            {
                originalSprite = null;
                originalSortingOrder = 0;
            }

            originalBodyType = rb != null ? rb.bodyType : RigidbodyType2D.Dynamic;
            originalEnemyMoveEnabled = enemyMove != null && enemyMove.enabled;

            hasOriginalSnapshot = true;
        }

        /// <summary>
        /// トリガー侵入時にプレイヤーの攻撃判定をチェックする
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isDead) return;
            if (!other.CompareTag(PLAYER_ATTACK_TAG)) return;

            TakeDamage(1);
        }

        /// <summary>
        /// ダメージを適用し、体力が0以下になれば死亡処理を行う
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (isDead) return;

            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// 死亡時の物理挙動、見た目、レイヤーの変更を行う
        /// </summary>
        private void Die()
        {
            if (isDead) return;
            isDead = true;

            if (deadSprite != null)
            {
                spriteRenderer.sprite = deadSprite;
            }

            if (enemyMove != null)
            {
                enemyMove.enabled = false;
            }

            ApplyDeadPhysics();

            UpdateCollidersForDeath();

            if (groundLayerHash != -1)
            {
                gameObject.layer = groundLayerHash;
            }
        }

        /// <summary>
        /// 死亡時の物理パラメータを適用する
        /// </summary>
        private void ApplyDeadPhysics()
        {
            if (rb == null) return;

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.freezeRotation = true;
            rb.WakeUp();
            
            rb.linearDamping = DEAD_LINEAR_DAMPING;
            rb.mass = DEAD_MASS;
        }

        /// <summary>
        /// 死亡時に不要な判定を無効化し、本体を実体化する
        /// </summary>
        private void UpdateCollidersForDeath()
        {
            Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
            
            foreach (var col in allColliders)
            {
                if (col.TryGetComponent<DamageSource>(out var ds))
                {
                    ds.enabled = false;
                    col.enabled = false;
                    continue;
                }

                col.isTrigger = false;
            }
        }

        /// <summary>
        /// 記録された状態に基づいて死亡状態を復元またはリセットする
        /// </summary>
        public void ApplyRecordedDeathState(bool shouldBeDead)
        {
            if (shouldBeDead)
            {
                if (isDead) return;
                Die();
            }
            else
            {
                if (!isDead) return;
                RestoreAliveForPlayback();
            }
        }

        /// <summary>
        /// 再生用に生存状態のパラメータを復元する
        /// </summary>
        private void RestoreAliveForPlayback()
        {
            CaptureOriginalSnapshot();

            isDead = false;
            currentHealth = maxHealth;

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = originalSprite;
                spriteRenderer.sortingOrder = originalSortingOrder;
            }

            if (enemyMove != null)
            {
                enemyMove.enabled = originalEnemyMoveEnabled;
            }

            if (rb != null)
            {
                rb.bodyType = originalBodyType;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            gameObject.layer = originalLayer;
        }
    }
}