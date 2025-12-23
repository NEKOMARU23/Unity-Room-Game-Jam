using UnityEngine;

namespace Main.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMove : MonoBehaviour
    {
        [Header("移動設定")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;

        [Header("接地判定設定")]
        [SerializeField] private LayerMask groundLayer; 
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
        [SerializeField] private float groundCheckOffset = 0.5f;

        private Rigidbody2D rb;
        private Vector2 moveInput = Vector2.zero;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }

        void FixedUpdate()
        {
            HandleMovement();
        }

        public void OnMoveInput(Vector2 input) => moveInput = input;

        public void DoJump()
        {
            if (IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        private void HandleMovement()
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

            if (moveInput.x > 0) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            else if (moveInput.x < 0) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        public bool IsGrounded()
        {
            Vector2 origin = (Vector2)transform.position + Vector2.down * groundCheckOffset;
            RaycastHit2D hit = Physics2D.BoxCast(origin, groundCheckSize, 0f, Vector2.down, 0.1f, groundLayer);
            
            return hit.collider != null;
        }

        /// <summary>
        /// シーンビューに判定用の箱を表示
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = IsGrounded() ? Color.green : Color.red;
            Vector2 origin = (Vector2)transform.position + Vector2.down * groundCheckOffset;
            Gizmos.DrawWireCube(origin, groundCheckSize);
        }
    }
}