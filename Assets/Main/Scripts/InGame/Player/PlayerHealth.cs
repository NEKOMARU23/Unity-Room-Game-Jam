using UnityEngine;
using Main.Scene;
using Main.InGame.Core;

namespace Main.InGame.Player
{
    /// <summary>
    /// プレイヤーの体力を管理し、ダメージ処理および死亡時のシーン遷移を制御する。
    /// 再生システムの状態に応じて初期化をスキップする。
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("ステータス")]
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float damageCooldown = 0.5f;

        private static int currentHealth;
        private static bool isDead = false;

        public static int CurrentHealth => currentHealth;

        private float lastDamageTime;

        private void Awake()
        {
            InitializeHealthState();
        }

        /// <summary>
        /// 再生中（ゴースト出現中）でない場合のみ、体力を初期状態に戻す
        /// </summary>
        private void InitializeHealthState()
        {
            if (IsReplaying()) return;

            currentHealth = maxHealth;
            isDead = false;
        }

        /// <summary>
        /// 現在システムが録画再生中かどうかを判定する
        /// </summary>
        private bool IsReplaying()
        {
            var playbackSystem = Object.FindAnyObjectByType<RecordingPlaybackSystem>();
            return playbackSystem != null && playbackSystem.IsPlaying;
        }

        /// <summary>
        /// 指定された量のダメージを適用する。クールタイム中は無視される。
        /// </summary>
        public void TakeDamage(int amount, string sourceName = "Unknown")
        {
            if (isDead) return;
            if (Time.time < lastDamageTime + damageCooldown) return;

            ApplyDamage(amount);
        }

        /// <summary>
        /// 実際に体力を減算し、0以下になった場合は死亡処理を実行する
        /// </summary>
        private void ApplyDamage(int amount)
        {
            currentHealth -= amount;
            lastDamageTime = Time.time;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// 死亡フラグを立て、ゲームオーバーシーンへ遷移する
        /// </summary>
        private void Die()
        {
            if (isDead) return;
            isDead = true;

            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadScene(SceneName.GameOver);
            }
        }
    }
}