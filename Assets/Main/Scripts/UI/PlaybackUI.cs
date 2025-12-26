using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace Main.Player
{
    public class PlaybackUI : MonoBehaviour
    {
        [Header("スプライト設定")]
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Vector2 normalSize = new Vector2(100, 100); // 追加
        [SerializeField] private Sprite playbackSprite;
        [SerializeField] private Vector2 playbackSize = new Vector2(100, 100); // 追加

        [Header("テキスト設定")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private string normalString = "READY";
        [SerializeField] private string playbackString = "PLAYING";

        private InputAction playbackAction;
        private bool isPlaying = false;

        private void Awake()
        {
            // キーをEに変更（提示されたコードに合わせました）
            playbackAction = new InputAction(binding: "<Keyboard>/e");
            
            UpdateUI();
        }

        private void OnEnable()
        {
            playbackAction.Enable();
            playbackAction.performed += OnTogglePlaybackUI;
        }

        private void OnDisable()
        {
            playbackAction.Disable();
            playbackAction.performed -= OnTogglePlaybackUI;
        }

        private void OnTogglePlaybackUI(InputAction.CallbackContext context)
        {
            isPlaying = !isPlaying;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (targetImage != null)
            {
                // スプライトとサイズを同時に切り替え
                targetImage.sprite = isPlaying ? playbackSprite : normalSprite;
                
                // RectTransformのサイズを更新
                targetImage.rectTransform.sizeDelta = isPlaying ? playbackSize : normalSize;
            }

            if (statusText != null)
            {
                statusText.text = isPlaying ? playbackString : normalString;
            }
        }
    }
}