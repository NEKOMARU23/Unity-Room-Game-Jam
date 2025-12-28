using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使う場合
using Main.InGame.Core;

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

        [Header("外部参照")]
        [SerializeField] private RecordingSystem recordingSystem;
        [SerializeField] private MonochromeChange monochromeChange;

        private bool lastIsRecording;
        private bool lastIsMonochrome;

        private void Awake()
        {
            if (recordingSystem == null) recordingSystem = FindFirstObjectByType<RecordingSystem>();
            if (monochromeChange == null) monochromeChange = FindFirstObjectByType<MonochromeChange>();

            // 初期状態の反映
            ForceRefresh();
        }

        private void Update()
        {
            bool isMonochrome = monochromeChange != null && monochromeChange.isMonochrome;
            bool isRecording = !isMonochrome && recordingSystem != null && recordingSystem.IsRecording;

            if (isRecording == lastIsRecording && isMonochrome == lastIsMonochrome) return;

            lastIsRecording = isRecording;
            lastIsMonochrome = isMonochrome;
            UpdateUI(isRecording);
        }

        private void ForceRefresh()
        {
            lastIsRecording = false;
            lastIsMonochrome = false;
            Update();
        }

        private void UpdateUI(bool isRecording)
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