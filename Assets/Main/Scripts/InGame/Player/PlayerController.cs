using UnityEngine;
using UnityEngine.InputSystem;
using Main.InGame.Core;

namespace Main.InGame.Player
{
    /// <summary>
    /// InputSystemからの入力を受け取り、移動・攻撃・録画・再生などの各システムへ命令を仲介する。
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private PlayerMove playerMove;
        private PlayerAnimation playerAnim;
        private MonochromeChange monochromeChange;
        private RecordingSystem recordingSystem;
        private RecordingPlaybackSystem playbackSystem;
        private MonochromeChangeEffect monochromeChangeEffect;

        private void Awake()
        {
            InitializeReferences();
        }

        /// <summary>
        /// 必要な各システムへの参照を取得しキャッシュする
        /// </summary>
        private void InitializeReferences()
        {
            playerMove = GetComponent<PlayerMove>();
            playerAnim = GetComponent<PlayerAnimation>();

            monochromeChange = FindAnyObjectByType<MonochromeChange>();
            recordingSystem = FindAnyObjectByType<RecordingSystem>();
            playbackSystem = FindAnyObjectByType<RecordingPlaybackSystem>();
            monochromeChangeEffect = FindAnyObjectByType<MonochromeChangeEffect>(FindObjectsInactive.Include);
        }

        public void OnMove(InputValue value)
        {
            if (playerMove == null) return;
            
            playerMove.OnMoveInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            if (playerMove == null || !value.isPressed) return;
            
            playerMove.DoJump();
        }

        public void OnAttack(InputValue value)
        {
            if (playerAnim == null || !value.isPressed) return;
            if (playerMove == null || !playerMove.IsGrounded()) return;

            playerAnim.PlayAttack();
        }

        /// <summary>
        /// 画面状態の切り替え（カラー/モノクロ・再生停止）を制御
        /// </summary>
        public void OnScreenChange(InputValue value)
        {
            if (!value.isPressed) return;

            if (playbackSystem != null && playbackSystem.IsPlaying)
            {
                StopAllSystems();
                return;
            }

            if (monochromeChange != null && monochromeChange.isMonochrome)
            {
                monochromeChange.DisableMono();
                return;
            }

            TryStartPlayback();
        }

        public void OnRecording(InputValue value)
        {
            if (!value.isPressed) return;

            // モノクロ状態や再生中は録画を制限
            if (monochromeChange != null && monochromeChange.isMonochrome) return;
            if (playbackSystem != null && playbackSystem.IsPlaying) return;

            ToggleRecording();
        }

        /// <summary>
        /// 再生、録画、モノクロの全システムをリセットしてカラーに戻す
        /// </summary>
        private void StopAllSystems()
        {
            if (playbackSystem != null) playbackSystem.StopPlayback();
            if (recordingSystem != null) recordingSystem.ResetRecording();
            if (monochromeChange != null) monochromeChange.DisableMono();
        }

        /// <summary>
        /// 録画クリップが存在する場合に再生処理を開始する
        /// </summary>
        private void TryStartPlayback()
        {
            if (recordingSystem == null || recordingSystem.LastClip == null) return;

            if (monochromeChange != null)
            {
                monochromeChange.EnableMono();
                if (monochromeChangeEffect != null)
                {
                    monochromeChangeEffect.gameObject.SetActive(true);
                }
            }

            if (playbackSystem != null)
            {
                playbackSystem.Play(recordingSystem.LastClip);
            }
        }

        private void ToggleRecording()
        {
            if (recordingSystem == null) return;

            if (recordingSystem.IsRecording)
            {
                recordingSystem.StopRecording();
            }
            else
            {
                recordingSystem.StartRecording();
            }
        }
    }
}