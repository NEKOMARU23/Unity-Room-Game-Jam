using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro; // TextMeshProを使う場合

namespace Main.Player
{
    public class RecordingUI : MonoBehaviour
    {
        [Header("スプライト設定")]
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite recordingSprite;

        [Header("テキスト設定")]
        [SerializeField] private TextMeshProUGUI statusText; // TextMeshProの場合
        // [SerializeField] private Text statusTextLegacy; // 標準Textを使う場合はこちら
        [SerializeField] private string normalString = "READY";
        [SerializeField] private string recordingString = "REC";

        private InputAction recordAction;
        private bool isRecording = false;

        private void Awake()
        {
            recordAction = new InputAction(binding: "<Keyboard>/q");
            
            // 初期状態の反映
            UpdateUI();
        }

        private void OnEnable()
        {
            recordAction.Enable();
            recordAction.performed += OnToggleRecordingUI;
        }

        private void OnDisable()
        {
            recordAction.Disable();
            recordAction.performed -= OnToggleRecordingUI;
        }

        private void OnToggleRecordingUI(InputAction.CallbackContext context)
        {
            isRecording = !isRecording;
            UpdateUI();
        }

        private void UpdateUI()
        {
            // スプライトの切り替え
            if (targetImage != null)
            {
                targetImage.sprite = isRecording ? recordingSprite : normalSprite;
            }

            // 文字の切り替え
            if (statusText != null)
            {
                statusText.text = isRecording ? recordingString : normalString;
            }
        }
    }
}