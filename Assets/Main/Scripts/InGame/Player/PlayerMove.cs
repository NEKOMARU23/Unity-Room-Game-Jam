using UnityEngine;

namespace Main.Player
{
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

        // flipX が false (右向き) の時の位置
        [SerializeField] private Vector2 groundCheckOffsetRight = new Vector2(0f, -0.5f);
        // flipX が true (左向き) の時の位置
        [SerializeField] private Vector2 groundCheckOffsetLeft = new Vector2(0f, -0.5f);

        private Rigidbody2D rb;
        private Vector2 moveInput = Vector2.zero;
        private PlayerAnimation playerAnim;
        private bool facingLeft;

        public bool IsFacingLeft => facingLeft;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            playerAnim = GetComponent<PlayerAnimation>();
            facingLeft = IsFacingLeftByRotation();
        }

        void FixedUpdate()
        {
            bool attacking = (playerAnim != null && playerAnim.IsAttacking());

            if (attacking)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

                // 入力から向きだけ決める（回転の適用はLateUpdateで行う）
                if (moveInput.x > 0)
                {
                    facingLeft = false;
                }
                else if (moveInput.x < 0)
                {
                    facingLeft = true;
                }
            }
        }

        private void LateUpdate()
        {
            // Rigidbody2D が物理更新で Transform回転(Z)を書き戻すため、描画直前にY回転を当て直す
            ApplyFacingRotation();
        }

        public void OnMoveInput(Vector2 input) => moveInput = input;
        
        public Vector2 GetVelocity() => rb != null ? rb.linearVelocity : Vector2.zero;
        public Vector2 GetCurrentMoveInput() => moveInput;

        public void DoJump()
        {
            if (playerAnim != null && playerAnim.IsAttacking()) return;

            if (IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        public bool IsGrounded()
        {
            // 向き（回転）を見て使用するオフセットを切り替える
            bool isLeft = facingLeft;
            Vector2 currentOffset = isLeft ? groundCheckOffsetLeft : groundCheckOffsetRight;
            Vector2 origin = (Vector2)transform.position + currentOffset;

            RaycastHit2D hit = Physics2D.BoxCast(origin, groundCheckSize, 0f, Vector2.down, 0.1f, groundLayer);
            return hit.collider != null;
        }

        private void OnDrawGizmos()
        {
            // 向きに応じた位置を計算（Editor上でも回転から判定）
            bool isLeft = Application.isPlaying ? facingLeft : IsFacingLeftByRotation();
            Vector2 currentOffset = isLeft ? groundCheckOffsetLeft : groundCheckOffsetRight;
            Vector2 origin = (Vector2)transform.position + currentOffset;

            bool grounded = IsGrounded();

            // 1. 塗りつぶしのボックス（半透明）
            Gizmos.color = grounded ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(origin, groundCheckSize);

            // 2. 枠線（くっきりした色）
            Gizmos.color = grounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(origin, groundCheckSize);

            // 3. 中心からの繋がりを示す線
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, origin);

            // 4. 判定の向き（下向き）
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(origin, Vector2.down * 0.2f);
        }

        private void ApplyFacingRotation()
        {
            var euler = transform.localEulerAngles;
            euler.y = facingLeft ? 180f : 0f;
            transform.localEulerAngles = euler;
        }

        private bool IsFacingLeftByRotation()
        {
            // 0/180 の近似で判定（他の角度を使わない前提）
            return Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, 180f)) < 1f;
        }
    }
}