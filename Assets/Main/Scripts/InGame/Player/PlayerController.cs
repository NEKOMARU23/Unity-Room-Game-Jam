using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerMove playerMove;

        void Awake()
        {
            playerMove = GetComponent<PlayerMove>();
        }

        public void OnMove(InputValue value)
        {
            if (playerMove != null)
            {
                Vector2 moveInput = value.Get<Vector2>();
                playerMove.OnMoveInput(moveInput);
            }
        }

        public void OnJump(InputValue value)
        {
            // value.isPressed はボタンが押された瞬間に true になります
            if (playerMove != null && value.isPressed)
            {
                playerMove.DoJump();
            }
        }

        public void OnRestart(InputValue value)
        {
            if (value.isPressed)
            {
                Debug.Log("Restart!");
            }
        }
    }
}