using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private string normalString = "READY";
        [SerializeField] private string recordingString = "REC";

        [Header("外部参照")]
        [SerializeField] private RecordingSystem recordingSystem;
        [SerializeField] private MonochromeChange monochromeChange;

        // 初期化が終わったかどうか
        private bool _isInitialized = false;
        private bool _lastIsRecording;
        private bool _lastIsMonochrome;

        private void Awake()
        {
            if (recordingSystem == null) recordingSystem = FindFirstObjectByType<RecordingSystem>();
            if (monochromeChange == null) monochromeChange = FindFirstObjectByType<MonochromeChange>();
        }

        private void Start()
        {
            // 開始時に強制的に1回更新する
            RefreshUI();
        }

        private void Update()
        {
            if (monochromeChange == null) return;

            // 今の状態を取得
            bool currentMono = monochromeChange.isMonochrome;
            bool currentRec = (recordingSystem != null) ? recordingSystem.IsRecording : false;
            
            // 録画UIが表示されるべき状態（モノクロじゃない、かつ録画中）
            bool shouldShowRecording = !currentMono && currentRec;

            // 初回、または状態が変わった時だけ更新
            if (!_isInitialized || shouldShowRecording != _lastIsRecording || currentMono != _lastIsMonochrome)
            {
                _isInitialized = true;
                _lastIsRecording = shouldShowRecording;
                _lastIsMonochrome = currentMono;

                UpdateUI(shouldShowRecording);
            }
        }

        private void RefreshUI()
        {
            // 現在の状態に関わらず、一度Updateを通すためのリセット
            _isInitialized = false;
        }

        private void UpdateUI(bool isRecording)
        {
            if (targetImage != null)
            {
                targetImage.sprite = isRecording ? recordingSprite : normalSprite;
                // 透明度を1にする（念のため）
                Color c = targetImage.color;
                c.a = 1f;
                targetImage.color = c;
            }

            if (statusText != null)
            {
                statusText.text = isRecording ? recordingString : normalString;
            }
        }
    }
}