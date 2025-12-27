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
                Debug.LogError("[DamageSource] MonochromeChange が見つかりません");
            }
            else
            {
                Debug.Log($"[DamageSource] MonochromeChange 取得成功 (InstanceID={_monoChange.GetInstanceID()})");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled) return;

            if (!other.CompareTag("Player")) return;

            Debug.Log($"[DamageSource] Player 接触 : {gameObject.name}");

            if (!CanDamage())
            {
                Debug.Log("[DamageSource] 無敵条件成立 → ダメージ無効");
                return;
            }

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

            Debug.Log(
                $"[DamageSource] 判定状態 => " +
                $"Eキー={isEPressed}, モノクロ={isWorldMono}"
            );

            return !(isEPressed || isWorldMono);
        }

        /// <summary>
        /// 実際にダメージを与える処理
        /// </summary>
        private void ExecuteDamage(Collider2D playerCollider)
        {
            var playerHealth = playerCollider.GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                Debug.LogWarning("[DamageSource] PlayerHealth が見つかりません");
                return;
            }

            Debug.Log(
                $"[DamageSource] ダメージ実行 " +
                $"(量={damageAmount}, 発生源={gameObject.name})"
            );

            playerHealth.TakeDamage(damageAmount, gameObject.name);
        }
    }
}
