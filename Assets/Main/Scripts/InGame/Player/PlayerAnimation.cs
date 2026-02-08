using UnityEngine;

namespace Main.InGame.Player
{
    /// <summary>
    /// プレイヤーのアニメーション制御および攻撃判定の有効化・位置調整を管理する。
    /// </summary>
    public class PlayerAnimation : MonoBehaviour
    {
        [Header("攻撃判定設定")]
        [SerializeField] private GameObject attackHitbox;
        [SerializeField] private Vector2 attackOffsetRight = new Vector2(0.5f, 0f);
        [SerializeField] private Vector2 attackOffsetLeft = new Vector2(-0.5f, 0f);

        private Animator anim;
        private PlayerMove playerMove;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            playerMove = GetComponent<PlayerMove>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            InitializeAttackHitbox();
        }

        private void Update()
        {
            if (!IsComponentsValid()) return;

            spriteRenderer.flipX = false;

            UpdateMovementParameters();
            UpdateAttackHitboxPosition();
        }

        /// <summary>
        /// 攻撃判定の初期状態設定
        /// </summary>
        private void InitializeAttackHitbox()
        {
            if (attackHitbox != null)
            {
                attackHitbox.SetActive(false);
            }
        }

        /// <summary>
        /// 必要なコンポーネントがすべて存在するかチェック
        /// </summary>
        private bool IsComponentsValid()
        {
            return anim != null && playerMove != null && spriteRenderer != null;
        }

        /// <summary>
        /// 速度や接地判定に基づいてアニメーターのパラメーターを更新
        /// </summary>
        private void UpdateMovementParameters()
        {
            bool isGrounded = playerMove.IsGrounded();

            if (!isGrounded)
            {
                anim.ResetTrigger("Attack");
            }

            float horizontalSpeed = Mathf.Abs(playerMove.GetVelocity().x);
            anim.SetFloat("Speed", horizontalSpeed);
            anim.SetBool("Jump", !isGrounded);
        }

        /// <summary>
        /// 向いている方向に応じて攻撃判定の位置とスケールを調整
        /// </summary>
        private void UpdateAttackHitboxPosition()
        {
            if (attackHitbox == null) return;

            bool isLeft = playerMove.IsFacingLeft;
            
            attackHitbox.transform.localPosition = isLeft ? attackOffsetLeft : attackOffsetRight;

            Vector3 localScale = attackHitbox.transform.localScale;
            float targetScaleX = Mathf.Abs(localScale.x);
            localScale.x = isLeft ? -targetScaleX : targetScaleX;
            attackHitbox.transform.localScale = localScale;
        }

        public void PlayAttack()
        {
            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }
        }

        public bool IsAttacking()
        {
            if (anim == null) return false;
            
            return anim.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        }

        public void ActivateAttack()
        {
            if (attackHitbox != null)
            {
                attackHitbox.SetActive(true);
            }
        }

        public void DeactivateAttack()
        {
            if (attackHitbox != null)
            {
                attackHitbox.SetActive(false);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;

            var pm = playerMove != null ? playerMove : GetComponent<PlayerMove>();
            bool isLeft = pm != null && pm.IsFacingLeft;

            Gizmos.color = Color.orange;
            Vector2 offset = isLeft ? attackOffsetLeft : attackOffsetRight;
            Vector3 worldPos = transform.position + (Vector3)offset;

            Gizmos.DrawWireSphere(worldPos, 0.2f);
            Gizmos.DrawLine(transform.position, worldPos);
        }
    }
}