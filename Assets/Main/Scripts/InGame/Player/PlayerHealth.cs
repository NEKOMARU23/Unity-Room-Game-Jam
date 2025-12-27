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
                Debug.Log($"<color=cyan>通常プレイ開始: HP {currentHealth}/{maxHealth} にリセット</color>");
            }
            else
            {
                Debug.Log($"<color=yellow>リプレイ中につきHPリセットをスキップ: 現在のHP {currentHealth}</color>");
            }
        }

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (isDead) return;
        //     if (Time.time < lastDamageTime + damageCooldown) return;
        //     if (other.transform.IsChildOf(this.transform)) return;

        //     if (other.TryGetComponent<DamageSource>(out var source))
        //     {
        //         if (source.enabled)
        //         {
        //             TakeDamage(source.DamageAmount, other.gameObject.name);
        //         }
        //     }
        // }

        public void TakeDamage(int amount, string sourceName = "Unknown")
        {
            if (isDead) return;

            currentHealth -= amount;
            lastDamageTime = Time.time;

            Debug.Log($"<color=red>ダメージ受領!</color> 発生源: {sourceName} | 残りHP: {currentHealth}");

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