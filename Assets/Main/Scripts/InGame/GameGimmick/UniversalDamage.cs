using UnityEngine;
using Main.Player;

namespace Main.InGame.GameGimmick
{
    /// <summary>
    /// 接触したプレイヤーに対してダメージを適用する汎用クラス
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class UniversalDamage : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private int damageAmount = 1;
        [SerializeField] private bool canHitMultipleTimes = true;

        private const string PLAYER_TAG = "Player";

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled) return;
            if (!other.CompareTag(PLAYER_TAG)) return;

            ProcessDamage(other);
        }

        /// <summary>
        /// ダメージ処理の実行と、設定に基づいたコンポーネントの無効化管理
        /// </summary>
        private void ProcessDamage(Collider2D playerCollider)
        {
            ExecuteDamage(playerCollider);

            if (!canHitMultipleTimes)
            {
                enabled = false;
            }
        }

        /// <summary>
        /// PlayerHealthコンポーネントを取得し、ダメージを適用する
        /// </summary>
        private void ExecuteDamage(Collider2D playerCollider)
        {
            if (playerCollider.TryGetComponent<PlayerHealth>(out var health))
            {
                health.TakeDamage(damageAmount, gameObject.name);
            }
        }
    }
}