using UnityEngine;
using Main.Scene;
using Main.Damage;
using Main.InGame.Core; // RecordingPlaybackSystem を参照するために追加

namespace Main.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("ステータス")]
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float damageCooldown = 0.5f;

        // 値を保持するために static にする
        private static int currentHealth;
        
        public static int CurrentHealth => currentHealth;
        private static bool isDead = false;

        private float lastDamageTime;

        void Awake()
        {
            // 再生システムが存在し、かつ「再生中」であれば、絶対に初期化しない
            var playbackSystem = Object.FindAnyObjectByType<RecordingPlaybackSystem>();
            bool isReplaying = playbackSystem != null && playbackSystem.IsPlaying;

            if (!isReplaying)
            {
                currentHealth = maxHealth;
                isDead = false;
            }
        }

        public void TakeDamage(int amount, string sourceName = "Unknown")
        {
            if (isDead) return;

            currentHealth -= amount;
            lastDamageTime = Time.time;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;

            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadScene(SceneName.Clear);
            }
        }
    }
}