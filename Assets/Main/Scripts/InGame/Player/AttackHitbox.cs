using UnityEngine;

namespace Main.Player
{
    public class AttackHitbox : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            // もし当たった相手が Enemy タグを持っていたら
            if (other.CompareTag("Enemy"))
            {
                // ここで敵のダメージ処理を呼ぶ
                
                // 例: 相手を消滅させる場合
                // Destroy(other.gameObject);
            }
        }
    }
}