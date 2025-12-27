using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Main.InGame.Core; // MonochromeChange を参照するため

namespace Main.Player
{
    public class PlaybackUI : MonoBehaviour
    {
        [Header("外部参照")]
        private MonochromeChange _monoChange;

        [Header("スプライト設定")]
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Vector2 normalSize = new Vector2(100, 100);
        [SerializeField] private Sprite playbackSprite;
        [SerializeField] private Vector2 playbackSize = new Vector2(100, 100);

        [Header("テキスト設定")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private string normalString = "READY";
        [SerializeField] private string playbackString = "PLAYING";

        // 前回の状態を覚えておくための変数（無駄な更新を避けるため）
        private bool _lastState = false;

        private void Awake()
        {
            _monoChange = FindFirstObjectByType<MonochromeChange>();
            
            // 初期状態の反映
            UpdateUI(false);
        }

        private void Update()
        {
            if (_monoChange == null) return;

            // MonochromeChange のフラグを直接監視
            bool currentState = _monoChange.isMonochrome;

            // 状態が変化したときだけUIを更新する
            if (currentState != _lastState)
            {
                _lastState = currentState;
                UpdateUI(currentState);
            }
        }

        private void UpdateUI(bool isMonochrome)
        {
            if (targetImage != null)
            {
                targetImage.sprite = isMonochrome ? playbackSprite : normalSprite;
                targetImage.rectTransform.sizeDelta = isMonochrome ? playbackSize : normalSize;
            }

            if (statusText != null)
            {
                statusText.text = isMonochrome ? playbackString : normalString;
            }
        }
    }
}