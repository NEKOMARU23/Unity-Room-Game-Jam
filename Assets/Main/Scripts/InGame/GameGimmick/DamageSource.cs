using UnityEngine;
using Main.InGame.Core;
using Main.InGame.Player;
using UnityEngine.InputSystem;

namespace Main.InGame.GameGimmick
{
    /// <summary>
    /// プレイヤーにダメージを与える判定を管理するクラス。
    /// 特定の入力状態や世界の状態でダメージの無効化を判定する。
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DamageSource : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private int damageAmount = 1;
        [SerializeField] private bool canHitMultipleTimes = true;

        private const string PLAYER_TAG = "Player";

        private MonochromeChange monoChange;
        private Keyboard currentKeyboard;

        /// <summary>
        /// ダメージ量の取得
        /// </summary>
        public int DamageAmount => damageAmount;

        private void Awake()
        {
            InitializeReferences();
        }

        /// <summary>
        /// 参照の初期化とキャッシュ
        /// </summary>
        private void InitializeReferences()
        {
            monoChange = FindFirstObjectByType<MonochromeChange>();
            currentKeyboard = Keyboard.current;

            if (monoChange == null)
            {
                Debug.LogError($"[DamageSource] {gameObject.name}: MonochromeChange がシーン内に見つかりません。");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enabled) return;
            if (!other.CompareTag(PLAYER_TAG)) return;
            if (!CanDamage()) return;

            ExecuteDamage(other);
            HandleHitCount();
        }

        /// <summary>
        /// ダメージ適用可否の判定。
        /// Eキー押下中、またはモノクロ化している場合は無効。
        /// </summary>
        private bool CanDamage()
        {
            bool isEPressed = currentKeyboard != null && currentKeyboard.eKey.isPressed;
            bool isWorldMono = monoChange != null && monoChange.isMonochrome;

            return !(isEPressed || isWorldMono);
        }

        /// <summary>
        /// 対象のPlayerHealthコンポーネントを介してダメージを適用
        /// </summary>
        private void ExecuteDamage(Collider2D playerCollider)
        {
            if (playerCollider.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(damageAmount, gameObject.name);
            }
        }

        /// <summary>
        /// 複数回ヒット設定に基づいた有効状態の制御
        /// </summary>
        private void HandleHitCount()
        {
            if (!canHitMultipleTimes)
            {
                enabled = false;
            }
        }
    }
}