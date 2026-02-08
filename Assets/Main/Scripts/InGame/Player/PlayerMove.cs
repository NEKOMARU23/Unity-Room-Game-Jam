using UnityEngine;

namespace Main.InGame.Player
{
    /// <summary>
    /// プレイヤーの物理移動、ジャンプ、および向き（回転）を管理する。
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerMove : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;

        [Header("接地判定（左右個別設定）")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
        [SerializeField] private Vector2 groundCheckOffsetRight = new Vector2(0f, -0.5f);
        [SerializeField] private Vector2 groundCheckOffsetLeft = new Vector2(0f, -0.5f);

        private const float FACING_THRESHOLD = 1f;
        private const float GROUND_CHECK_DISTANCE = 0.1f;
        private const float Y_ROTATION_LEFT = 180f;
        private const float Y_ROTATION_RIGHT = 0f;

        private Rigidbody2D rb;
        private Vector2 moveInput = Vector2.zero;
        private PlayerAnimation playerAnim;
        private bool facingLeft;

        public bool IsFacingLeft => facingLeft;

        private void Awake()
        {
            InitializeComponents();
            facingLeft = IsFacingLeftByRotation();
        }

        private void FixedUpdate()
        {
            UpdateMovement();
            UpdateFacingDirection();
        }

        private void LateUpdate()
        {
            ApplyFacingRotation();
        }

        public void OnMoveInput(Vector2 input) => moveInput = input;
        public Vector2 GetVelocity() => rb != null ? rb.linearVelocity : Vector2.zero;
        public Vector2 GetCurrentMoveInput() => moveInput;

        /// <summary>
        /// コンポーネントの初期化と物理設定
        /// </summary>
        private void InitializeComponents()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            playerAnim = GetComponent<PlayerAnimation>();
        }

        /// <summary>
        /// 物理挙動に基づいた座標移動
        /// </summary>
        private void UpdateMovement()
        {
            bool isAttacking = (playerAnim != null && playerAnim.IsAttacking());
            float targetVelocityX = isAttacking ? 0f : moveInput.x * moveSpeed;

            rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
        }

        /// <summary>
        /// 入力値に基づく向きフラグの更新
        /// </summary>
        private void UpdateFacingDirection()
        {
            if (playerAnim != null && playerAnim.IsAttacking()) return;
            if (Mathf.Approximately(moveInput.x, 0f)) return;

            facingLeft = moveInput.x < 0;
        }

        /// <summary>
        /// ジャンプ処理の実行。攻撃中や非接地時は無視される。
        /// </summary>
        public void DoJump()
        {
            if (playerAnim != null && playerAnim.IsAttacking()) return;
            if (!IsGrounded()) return;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        /// <summary>
        /// 現在の足元が地面と接触しているかを判定する。
        /// </summary>
        public bool IsGrounded()
        {
            Vector2 currentOffset = facingLeft ? groundCheckOffsetLeft : groundCheckOffsetRight;
            Vector2 origin = (Vector2)transform.position + currentOffset;

            RaycastHit2D hit = Physics2D.BoxCast(origin, groundCheckSize, 0f, Vector2.down, GROUND_CHECK_DISTANCE, groundLayer);
            return hit.collider != null;
        }

        /// <summary>
        /// 物理演算の更新後に、フラグに基づいた描画向き（Y回転）を適用する。
        /// </summary>
        private void ApplyFacingRotation()
        {
            Vector3 euler = transform.localEulerAngles;
            euler.y = facingLeft ? Y_ROTATION_LEFT : Y_ROTATION_RIGHT;
            transform.localEulerAngles = euler;
        }

        private bool IsFacingLeftByRotation()
        {
            return Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, Y_ROTATION_LEFT)) < FACING_THRESHOLD;
        }

        private void OnDrawGizmos()
        {
            bool isLeft = Application.isPlaying ? facingLeft : IsFacingLeftByRotation();
            Vector2 currentOffset = isLeft ? groundCheckOffsetLeft : groundCheckOffsetRight;
            Vector2 origin = (Vector2)transform.position + currentOffset;
            bool grounded = IsGrounded();

            Gizmos.color = grounded ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(origin, groundCheckSize);

            Gizmos.color = grounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(origin, groundCheckSize);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, origin);

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(origin, Vector2.down * 0.2f);
        }
    }
}