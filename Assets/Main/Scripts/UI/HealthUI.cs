using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Main.InGame.Player;

namespace Main.InGame.UI
{
    /// <summary>
    /// PlayerHealthの現在の体力を監視し、ハートのスプライトを更新するUIクラス。
    /// </summary>
    public class HealthUI : MonoBehaviour
    {
        [Header("スプライト設定")]
        [SerializeField] private Sprite fullHeartSprite;
        [SerializeField] private Sprite emptyHeartSprite;

        [Header("UI要素（左から順に配列へ格納）")]
        [SerializeField] private List<Image> heartImages;

        private int lastHealth = -1;

        private void Update()
        {
            MonitorHealthChange();
        }

        /// <summary>
        /// 体力の変化を監視し、変更があった場合のみ表示を更新する
        /// </summary>
        private void MonitorHealthChange()
        {
            int currentHealth = PlayerHealth.CurrentHealth;

            if (currentHealth == lastHealth) return;

            lastHealth = currentHealth;
            UpdateHeartIcons(currentHealth);
        }

        /// <summary>
        /// 現在の体力値に基づいて、リスト内の各Imageのスプライトを差し替える
        /// </summary>
        private void UpdateHeartIcons(int currentHealth)
        {
            for (int i = 0; i < heartImages.Count; i++)
            {
                if (heartImages[i] == null) continue;

                bool isActive = i < currentHealth;
                heartImages[i].sprite = isActive ? fullHeartSprite : emptyHeartSprite;
            }
        }
    }
}