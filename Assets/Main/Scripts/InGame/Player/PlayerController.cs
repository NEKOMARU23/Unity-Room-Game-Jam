using System.Runtime.CompilerServices;
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

        void Awake()
        {
            playerMove = GetComponent<PlayerMove>();
            playerAnim = GetComponent<PlayerAnimation>();
            monochromeChange = FindAnyObjectByType<MonochromeChange>();
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
                if (monochromeChange.isMonochrome == true)
                {
                    monochromeChange.DisableMono();
                }
                else
                {
                    monochromeChange.EnableMono();
                }
            }
        }
    }
}