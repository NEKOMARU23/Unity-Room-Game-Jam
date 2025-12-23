using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using Main.InGame.Core;

namespace Main.Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerMove playerMove;
        private MonochromeChange monochromeChange;

        void Awake()
        {
            playerMove = GetComponent<PlayerMove>();
            monochromeChange = FindAnyObjectByType<MonochromeChange>();
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