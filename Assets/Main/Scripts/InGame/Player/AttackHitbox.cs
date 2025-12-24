using UnityEngine;

namespace Main.Player
{
    public class AttackHitbox : MonoBehaviour
    {
        [Header("位置設定（親のFlipXに同期）")]
        [SerializeField] private Vector2 offsetRight = new Vector2(0.5f, 0f);
        [SerializeField] private Vector2 offsetLeft = new Vector2(-0.5f, 0f);

        private SpriteRenderer parentSprite;

        void Awake()
        {
            // 親（Player本体）の SpriteRenderer を取得
            parentSprite = GetComponentInParent<SpriteRenderer>();
        }

        void Update()
        {
            // 親の向きに合わせて自分の位置を更新
            if (parentSprite != null)
            {
                transform.localPosition = parentSprite.flipX ? offsetLeft : offsetRight;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // もし当たった相手が Enemy タグを持っていたら
            if (other.CompareTag("Enemy"))
            {                
                // ここで敵のダメージ処理を呼ぶ（例）
                // var enemy = other.GetComponent<EnemyHealth>();
                // if (enemy != null) enemy.TakeDamage(10);
            }
        }

        // デバッグ用：攻撃範囲をオレンジ色の球で表示
        private void OnDrawGizmos()
        {
            if (parentSprite == null) parentSprite = GetComponentInParent<SpriteRenderer>();
            
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.6f); // オレンジ色
            Vector2 currentOffset = (parentSprite != null && parentSprite.flipX) ? offsetLeft : offsetRight;
            
            // 親の位置からの相対座標を表示
            if (transform.parent != null)
            {
                Vector3 worldPos = transform.parent.position + (Vector3)currentOffset;
                Gizmos.DrawSphere(worldPos, 0.2f);
                Gizmos.DrawLine(transform.parent.position, worldPos);
            }
        }
    }
}