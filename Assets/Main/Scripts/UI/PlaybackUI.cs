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
        [SerializeField] private Sprite playbackSprite;

        [Header("テキスト設定")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private string normalString = "READY";
        [SerializeField] private string playbackString = "PLAYING";

        private InputAction playbackAction;
        private bool isPlaying = false;

        private void Awake()
        {
            // Rキーで再生UIを切り替えるように設定
            playbackAction = new InputAction(binding: "<Keyboard>/e");
            
            // 初期状態の反映
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
            // スプライトの切り替え
            if (targetImage != null)
            {
                targetImage.sprite = isPlaying ? playbackSprite : normalSprite;
            }

            // 文字の切り替え
            if (statusText != null)
            {
                statusText.text = isPlaying ? playbackString : normalString;
            }
        }
    }
}