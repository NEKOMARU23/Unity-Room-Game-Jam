using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Main.InGame.Core;

namespace Main.InGame.UI
{
    /// <summary>
    /// モノクロ（再生）状態に応じて、UIのスプライト、サイズ、およびテキストを切り替える。
    /// </summary>
    public class PlaybackUI : MonoBehaviour
    {
        [Header("外部参照")]
        [SerializeField] private Image targetImage;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("スプライト設定")]
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Vector2 normalSize = new Vector2(100, 100);
        [SerializeField] private Sprite playbackSprite;
        [SerializeField] private Vector2 playbackSize = new Vector2(100, 100);

        [Header("テキスト設定")]
        [SerializeField] private string normalString = "READY";
        [SerializeField] private string playbackString = "PLAYING";

        private MonochromeChange monoChange;
        private bool lastState;

        private void Awake()
        {
            InitializeReferences();
            UpdateVisuals(false);
        }

        private void Update()
        {
            MonitorStateChange();
        }

        /// <summary>
        /// 必要なシステムへの参照を初期化する
        /// </summary>
        private void InitializeReferences()
        {
            monoChange = FindFirstObjectByType<MonochromeChange>();
        }

        /// <summary>
        /// システムのモノクロフラグを監視し、変更があった場合のみUIを更新する
        /// </summary>
        private void MonitorStateChange()
        {
            if (monoChange == null) return;

            bool currentState = monoChange.isMonochrome;
            if (currentState == lastState) return;

            lastState = currentState;
            UpdateVisuals(currentState);
        }

        /// <summary>
        /// 現在の状態に基づいてUIの表示を切り替える
        /// </summary>
        private void UpdateVisuals(bool isMonochrome)
        {
            UpdateImage(isMonochrome);
            UpdateText(isMonochrome);
        }

        private void UpdateImage(bool isMonochrome)
        {
            if (targetImage == null) return;

            targetImage.sprite = isMonochrome ? playbackSprite : normalSprite;
            targetImage.rectTransform.sizeDelta = isMonochrome ? playbackSize : normalSize;
        }

        private void UpdateText(bool isMonochrome)
        {
            if (statusText == null) return;

            statusText.text = isMonochrome ? playbackString : normalString;
        }
    }
}