using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Main.InGame.Core;

namespace Main.InGame.UI
{
    /// <summary>
    /// 録画システムの状態を監視し、録画中（REC）と待機中（READY）の表示を切り替える。
    /// モノクロ状態（再生中）の場合は表示を更新しない。
    /// </summary>
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

        private bool isInitialized;
        private bool lastIsRecording;
        private bool lastIsMonochrome;

        private void Awake()
        {
            InitializeReferences();
        }

        private void Start()
        {
            RefreshUI();
        }

        private void Update()
        {
            if (monochromeChange == null) return;

            UpdateUIState();
        }

        /// <summary>
        /// 必要な参照をキャッシュする
        /// </summary>
        private void InitializeReferences()
        {
            if (recordingSystem == null)
            {
                recordingSystem = FindFirstObjectByType<RecordingSystem>();
            }

            if (monochromeChange == null)
            {
                monochromeChange = FindFirstObjectByType<MonochromeChange>();
            }
        }

        /// <summary>
        /// 現在の状態を監視し、変化があった場合のみUIを更新する
        /// </summary>
        private void UpdateUIState()
        {
            bool currentMono = monochromeChange.isMonochrome;
            bool currentRec = recordingSystem != null && recordingSystem.IsRecording;
            bool shouldShowRecording = !currentMono && currentRec;

            // 状態が変化していない場合は処理をスキップ
            if (isInitialized && 
                shouldShowRecording == lastIsRecording && 
                currentMono == lastIsMonochrome) return;

            ApplyState(shouldShowRecording, currentMono);
        }

        /// <summary>
        /// 内部状態を記録し、実際の表示に反映する
        /// </summary>
        private void ApplyState(bool shouldShowRecording, bool currentMono)
        {
            isInitialized = true;
            lastIsRecording = shouldShowRecording;
            lastIsMonochrome = currentMono;

            UpdateVisuals(shouldShowRecording);
        }

        private void RefreshUI()
        {
            isInitialized = false;
        }

        /// <summary>
        /// スプライト、カラー、テキストの表示を切り替える
        /// </summary>
        private void UpdateVisuals(bool isRecording)
        {
            if (targetImage != null)
            {
                targetImage.sprite = isRecording ? recordingSprite : normalSprite;
                SetImageAlpha(1f);
            }

            if (statusText != null)
            {
                statusText.text = isRecording ? recordingString : normalString;
            }
        }

        private void SetImageAlpha(float alpha)
        {
            if (targetImage == null) return;

            Color color = targetImage.color;
            color.a = alpha;
            targetImage.color = color;
        }
    }
}