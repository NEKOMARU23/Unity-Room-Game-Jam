using UnityEngine;
using UnityEngine.InputSystem;
using Main.InGame.Core;

namespace Main.Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerMove playerMove;
        private PlayerAnimation playerAnim;
        private MonochromeChange monochromeChange;
        private RecordingSystem recordingSystem;
        private RecordingPlaybackSystem playbackSystem;

        void Awake()
        {
            playerMove = GetComponent<PlayerMove>();
            playerAnim = GetComponent<PlayerAnimation>();
            monochromeChange = FindAnyObjectByType<MonochromeChange>();
            recordingSystem = FindAnyObjectByType<RecordingSystem>();
            playbackSystem = FindAnyObjectByType<RecordingPlaybackSystem>();
        }



        // 横移動 (A, Dキー)
        public void OnMove(InputValue value)
        {
            if (playerMove != null)
                playerMove.OnMoveInput(value.Get<Vector2>());
        }

        // ジャンプ (Spaceキー)
        public void OnJump(InputValue value)
        {
            if (playerMove != null && value.isPressed)
            {
                playerMove.DoJump();
            }
        }

        public void OnAttack(InputValue value)
        {
            if (playerAnim != null && value.isPressed)
            {
                // PlayerMoveの接地判定をチェック
                if (playerMove != null && playerMove.IsGrounded())
                {
                    playerAnim.PlayAttack();
                }
            }
        }

        public void OnScreenChange(InputValue value)
        {
            if (value.isPressed)
            {
                // 再生中は「停止してカラーに戻す」
                if (playbackSystem != null && playbackSystem.IsPlaying)
                {
                    playbackSystem.StopPlayback();
                    if (recordingSystem != null) recordingSystem.ResetRecording();
                    if (monochromeChange != null) monochromeChange.DisableMono();
                    return;
                }

                // 既に白黒だけが有効になっている場合は解除
                if (monochromeChange != null && monochromeChange.isMonochrome)
                {
                    monochromeChange.DisableMono();
                    return;
                }

                // 録画があるときだけ白黒＋再生開始
                if (recordingSystem == null || recordingSystem.LastClip == null) return;

                if (monochromeChange != null) monochromeChange.EnableMono();
                if (playbackSystem != null) playbackSystem.Play(recordingSystem.LastClip);
            }
        }

        // Input Actions に "Record" を作って Q を割り当てる場合はこれを呼べます
        public void OnRecording(InputValue value)
        {
            if (value.isPressed)
            {
                // 再生中（モノクロ）に録画開始/停止されると状態が壊れやすいので無効化
                if (monochromeChange != null && monochromeChange.isMonochrome) return;
                if (playbackSystem != null && playbackSystem.IsPlaying) return;
                ToggleRecording();
            }
        }

        private void ToggleRecording()
        {
            if (recordingSystem == null) return;

            if (recordingSystem.IsRecording) recordingSystem.StopRecording();
            else recordingSystem.StartRecording();
        }
    }
}