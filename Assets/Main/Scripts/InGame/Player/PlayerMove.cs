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
        private SpriteRenderer spriteRenderer;
        private Vector2 moveInput = Vector2.zero;
        private PlayerAnimation playerAnim;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerAnim = GetComponent<PlayerAnimation>();
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

                // SpriteRendererの flipX を使用して反転
                if (moveInput.x > 0) 
                {
                    spriteRenderer.flipX = false;
                }
                else if (moveInput.x < 0) 
                {
                    spriteRenderer.flipX = true;
                }
            }
        }

        public void OnMoveInput(Vector2 input) => moveInput = input;
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
            // flipX の状態を見て使用するオフセットを切り替える
            Vector2 currentOffset = spriteRenderer.flipX ? groundCheckOffsetLeft : groundCheckOffsetRight;
            Vector2 origin = (Vector2)transform.position + currentOffset;

            RaycastHit2D hit = Physics2D.BoxCast(origin, groundCheckSize, 0f, Vector2.down, 0.1f, groundLayer);
            return hit.collider != null;
        }

        private void OnDrawGizmos()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;

            // 向きに応じた位置を計算
            Vector2 currentOffset = spriteRenderer.flipX ? groundCheckOffsetLeft : groundCheckOffsetRight;
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
    }
}