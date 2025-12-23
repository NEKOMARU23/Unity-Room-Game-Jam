using UnityEngine;

namespace Main.Player
{
    /// <summary>
    /// プレイヤーの移動処理のみを担当するクラス
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMove : MonoBehaviour
    {
        [Header("移動設定")]
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D rb;
        private Vector2 moveInput = Vector2.zero;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            // 重力などの設定をコードから保証（任意）
            rb.gravityScale = 1f; 
            rb.freezeRotation = true; // 勝手に転ばないように回転を固定
        }

        void FixedUpdate()
        {
            HandleMovement();
        }

        /// <summary>
        /// 外部（PlayerControllerなど）から移動入力を受け取る
        /// </summary>
        public void OnMoveInput(Vector2 input)
        {
            moveInput = input;
        }

        /// <summary>
        /// 横移動処理と向きの制御
        /// </summary>
        private void HandleMovement()
        {
            // 現在の垂直速度（重力）を維持したまま、水平速度を更新
            Vector2 targetVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = targetVelocity;

            // 移動方向に基づいて向き（左右）を反転
            if (moveInput.x > 0)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (moveInput.x < 0)
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }

        /// <summary>
        /// 速度をゼロにする（必要に応じて呼び出し）
        /// </summary>
        public void Stop()
        {
            moveInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }
}