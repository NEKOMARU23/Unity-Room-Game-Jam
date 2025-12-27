using UnityEngine;
using Main.Player; // PlayerHealth を参照

namespace Main.Damage
{
    /// <summary>
    /// 接触したプレイヤーに即座にダメージを与えるシンプルなクラス
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class UniversalDam : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private int damageAmount = 1;
        [Tooltip("一度当たったらこのコンポーネントを無効化するか（単発トラップ用）")]
        [SerializeField] private bool canHitMultipleTimes = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            // このコンポーネントが無効なら処理しない
            if (!enabled) return;

            // プレイヤーに当たったかチェック
            if (other.CompareTag("Player"))
            {
                ExecuteDamage(other);

                // 単発設定ならスクリプトをOFFにする
                if (!canHitMultipleTimes)
                {
                    enabled = false;
                }
            }
        }

        private void ExecuteDamage(Collider2D playerCollider)
        {
            var health = playerCollider.GetComponent<PlayerHealth>();
            if (health != null)
            {
                // ダメージ実行（発生源として自分のオブジェクト名を渡す）
                health.TakeDamage(damageAmount, gameObject.name);
            }
        }
    }
}