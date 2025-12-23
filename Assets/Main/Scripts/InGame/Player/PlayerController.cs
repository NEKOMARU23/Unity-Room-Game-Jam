using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerMove playerMove;
        private PlayerAnimation playerAnim;

        void Awake()
        {
            playerMove = GetComponent<PlayerMove>();
            playerAnim = GetComponent<PlayerAnimation>();
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
                // Trigger型なので1回呼ぶだけでOK
                playerAnim.PlayAttack();
            }
        }
    }
}