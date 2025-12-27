using UnityEngine;
using Main.InGame.Core;     // MonochromeChange
using Main.Player;          // PlayerHealth
using UnityEngine.InputSystem;

namespace Main.Damage
{
    [RequireComponent(typeof(Collider2D))]
    public class DamageSource : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private int damageAmount = 1;
        [SerializeField] private bool canHitMultipleTimes = true;

        public int DamageAmount => damageAmount;

        private MonochromeChange _monoChange;

        private void Awake()
        {
            _monoChange = FindFirstObjectByType<MonochromeChange>();

            if (_monoChange == null)
            {
                // 設定ミス防止のため、初期化時のエラーログのみ残しています
                Debug.LogError($"[DamageSource] {gameObject.name}: MonochromeChange がシーン内に見つかりません。");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled) return;

            if (!other.CompareTag("Player")) return;

            // 無敵条件（Eキー押下中 または モノクロ世界）なら処理を中断
            if (!CanDamage()) return;

            ExecuteDamage(other);

            if (!canHitMultipleTimes)
            {
                enabled = false;
            }
        }

        /// <summary>
        /// ダメージを与えてよいかどうかの最終判定
        /// </summary>
        private bool CanDamage()
        {
            bool isEPressed =
                Keyboard.current != null &&
                Keyboard.current.eKey.isPressed;

            bool isWorldMono =
                _monoChange != null &&
                _monoChange.isMonochrome;

            // いずれかの条件を満たしていれば無敵状態（CanDamage = false）
            return !(isEPressed || isWorldMono);
        }

        /// <summary>
        /// 実際にダメージを与える処理
        /// </summary>
        private void ExecuteDamage(Collider2D playerCollider)
        {
            var playerHealth = playerCollider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount, gameObject.name);
            }
        }
    }
}