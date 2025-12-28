using UnityEngine;

namespace Main.Player
{
    public class AttackHitbox : MonoBehaviour
    {
        private SpriteRenderer parentSprite;

        void Awake()
        {
            // 親（Player本体）の SpriteRenderer を取得
            parentSprite = GetComponentInParent<SpriteRenderer>();
        }

        void Update()
        {
            
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
    }
}