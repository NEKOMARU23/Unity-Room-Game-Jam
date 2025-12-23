using UnityEngine;

namespace Main.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMove : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;

        [Header("接地判定")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.1f);
        [SerializeField] private float groundCheckOffset = 0.5f;

        private Rigidbody2D rb;
        private Vector2 moveInput = Vector2.zero;
        private bool wasGrounded; // 前フレームの接地状態を記憶
        private PlayerAnimation playerAnim;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            playerAnim = GetComponent<PlayerAnimation>();
        }

        void FixedUpdate()
        {
            // 1. 攻撃中かどうかのチェック
            // PlayerAnimationコンポーネントを取得して、現在Attackステートかを確認
            PlayerAnimation animScript = GetComponent<PlayerAnimation>();
            bool attacking = (animScript != null && animScript.IsAttacking());

            if (attacking)
            {
                // 攻撃中は横移動速度を0にして、向きも変えない
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            else
            {
                // 攻撃中でない時のみ、通常の移動処理を行う
                rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

                // キャラクターの向きを更新
                if (moveInput.x > 0) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                else if (moveInput.x < 0) transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            // --- 地面判定のデバッグログ ---
            bool currentGrounded = IsGrounded();
            if (currentGrounded != wasGrounded)
            {
                if (currentGrounded)
                    Debug.Log("<color=green>接地しました</color>");
                else
                    Debug.Log("<color=yellow>空中へ離れました</color>");

                wasGrounded = currentGrounded;
            }
        }

        public void OnMoveInput(Vector2 input) => moveInput = input;
        public Vector2 GetCurrentMoveInput() => moveInput;

        public void DoJump()
        {
            if (IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                Debug.Log("<color=cyan>ジャンプ実行！</color>");
            }
            else
            {
                Debug.Log("<color=red>接地していないためジャンプできません</color>");
            }
        }

        public bool IsGrounded()
        {
            Vector2 origin = (Vector2)transform.position + Vector2.down * groundCheckOffset;
            RaycastHit2D hit = Physics2D.BoxCast(origin, groundCheckSize, 0f, Vector2.down, 0.1f, groundLayer);
            return hit.collider != null;
        }

        // シーンビューに判定用の枠を表示（実行中、接地すると緑色になります）
        private void OnDrawGizmos()
        {
            Gizmos.color = IsGrounded() ? Color.green : Color.red;
            Vector2 origin = (Vector2)transform.position + Vector2.down * groundCheckOffset;
            Gizmos.DrawWireCube(origin, groundCheckSize);
        }
    }
}