using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Player
{
    /// <summary>
    /// プレイヤーのメインコントローラー
    /// 入力を受け取り、PlayerMoveに伝える役割のみを担当
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        private PlayerMove playerMove;

        void Awake()
        {
            // 同じGameObjectに付いているPlayerMoveを取得
            playerMove = GetComponent<PlayerMove>();
        }

        /// <summary>
        /// PlayerInputから呼び出される移動入力のコールバック
        /// Input Action の名前が "Move" の場合に呼ばれる
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            if (playerMove != null)
            {
                // 入力値（Vector2）を読み取ってPlayerMoveに渡す
                Vector2 moveInput = context.ReadValue<Vector2>();
                playerMove.OnMoveInput(moveInput);
            }
        }

        /// <summary>
        /// PlayerInputから呼び出されるリスタート入力のコールバック
        /// Input Action の名前が "Restart" の場合に呼ばれる
        /// </summary>
        public void OnRestart(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // シンプルなシーンリロードによるリスタート例
                // もしPlayerMoveにResetPositionが残っているならそれを呼ぶ形でもOK
                Debug.Log("Restart入力が検知されました");
            }
        }
    }
}