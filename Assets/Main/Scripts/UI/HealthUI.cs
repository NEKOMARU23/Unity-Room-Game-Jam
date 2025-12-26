using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Main.Player
{
    public class HealthUI : MonoBehaviour
    {
        [Header("スプライト設定")]
        [SerializeField] private Sprite fullHeartSprite;  // 満タンの画像
        [SerializeField] private Sprite emptyHeartSprite; // 空の画像

        [Header("UI要素（左から順にドラッグ）")]
        [SerializeField] private List<Image> heartImages; 

        private void Update()
        {
            // PlayerHealthのstatic変数 currentHealth を参照してUIを更新
            UpdateHealthUI(PlayerHealth.CurrentHealth);
        }

        private void UpdateHealthUI(int currentHealth)
        {
            for (int i = 0; i < heartImages.Count; i++)
            {
                if (i < currentHealth)
                {
                    heartImages[i].sprite = fullHeartSprite;
                }
                else
                {
                    heartImages[i].sprite = emptyHeartSprite;
                }
            }
        }
    }
}