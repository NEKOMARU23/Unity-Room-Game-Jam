using UnityEngine;
using Main.Scene;
using Main.Damage;

namespace Main.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("ステータス")]
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float damageCooldown = 0.5f; // ダメージ後の無敵時間

        private int currentHealth;
        private float lastDamageTime;
        private bool isDead = false;

        void Awake()
        {
            currentHealth = maxHealth;
            Debug.Log($"<color=cyan>PlayerHealth初期化: HP {currentHealth}/{maxHealth}</color>");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 1. 死亡中なら何もしない
            if (isDead) return;

            // 2. 無敵時間（クールタイム）中なら何もしない
            if (Time.time < lastDamageTime + damageCooldown) return;

            // 3. 【自爆防止】当たった相手が自分自身、または自分の子供（AttackRangeなど）なら無視
            if (other.transform.IsChildOf(this.transform)) return;

            // 4. ダメージ源（DamageSource）を持っているかチェック
            if (other.TryGetComponent<DamageSource>(out var source))
            {
                // 5. 相手のダメージ設定が有効（敵が生きている等）な場合のみ実行
                if (source.enabled)
                {
                    TakeDamage(source.DamageAmount, other.gameObject.name);
                }
            }
        }

        // 引数に「何に当たったか」を追加してログを分かりやすく
        public void TakeDamage(int amount, string sourceName = "Unknown")
        {
            if (isDead) return;

            currentHealth -= amount;
            lastDamageTime = Time.time; // ダメージを受けた時間を記録

            Debug.Log($"<color=red>ダメージ受領!</color> 発生源: {sourceName} | ダメージ量: {amount} | 残りHP: {currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;

            Debug.Log("<color=yellow><b>プレイヤー死亡：リザルトシーンへ遷移します</b></color>");

            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadScene(SceneName.Clear);
            }
            else
            {
                Debug.LogWarning("SceneControllerが見つかりません。遷移をスキップしました。");
            }
        }
    }
}