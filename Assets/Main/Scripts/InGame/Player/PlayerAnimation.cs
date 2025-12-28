using UnityEngine;

namespace Main.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator anim;
        private PlayerMove playerMove;
        private SpriteRenderer spriteRenderer;

        [Header("攻撃判定設定")]
        [SerializeField] private GameObject attackHitbox;
        // flipX が false (右向き) の時の位置
        [SerializeField] private Vector2 attackOffsetRight = new Vector2(0.5f, 0f);
        // flipX が true (左向き) の時の位置
        [SerializeField] private Vector2 attackOffsetLeft = new Vector2(-0.5f, 0f);

        void Awake()
        {
            anim = GetComponent<Animator>();
            playerMove = GetComponent<PlayerMove>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // 初期状態で攻撃判定を消しておく
            if (attackHitbox != null) attackHitbox.SetActive(false);
        }

        void Update()
        {
            if (anim == null || playerMove == null || spriteRenderer == null) return;

            // 反転は回転で行うため、flipX は常に無効化しておく（アニメーション等で書き換わるのを防ぐ）
            spriteRenderer.flipX = false;

            // 1. 空中にいる間は攻撃トリガーをリセット
            if (!playerMove.IsGrounded())
            {
                anim.ResetTrigger("Attack");
            }

            // --- 修正点：Rigidbodyの速度を取得してアニメーションに反映 ---
            // 入力値(moveInput)ではなく、物理的な移動速度(linearVelocity)の絶対値を使用
            float horizontalSpeed = Mathf.Abs(playerMove.GetVelocity().x);
            anim.SetFloat("Speed", horizontalSpeed);

            // 2. 接地判定を Jump パラメーターに反映
            bool isInAir = !playerMove.IsGrounded();
            anim.SetBool("Jump", isInAir);

            // 3. 向きに合わせて攻撃判定の位置を更新
            UpdateAttackHitboxPosition();
        }

        private void UpdateAttackHitboxPosition()
        {
            if (attackHitbox == null) return;

            bool isLeft = playerMove.IsFacingLeft;

            // 1. まずコライダーが付いているオブジェクトの位置をずらす
            Vector2 currentOffset = isLeft ? attackOffsetLeft : attackOffsetRight;
            attackHitbox.transform.localPosition = currentOffset;

            // 2. コライダーの向きも反転させる（横長の判定などに対応）
            // オブジェクトのローカルスケールのXを書き換えて向きを合わせる
            Vector3 localScale = attackHitbox.transform.localScale;
            localScale.x = isLeft ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            attackHitbox.transform.localScale = localScale;
        }

        public void PlayAttack()
        {
            if (anim != null) anim.SetTrigger("Attack");
        }

        public bool IsAttacking()
        {
            if (anim == null) return false;
            // Animatorの現在のステートが "Attack" という名前の時
            return anim.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        }

        // --- アニメーションイベントから呼ばれるメソッド ---

        public void ActivateAttack()
        {
            if (attackHitbox != null) attackHitbox.SetActive(true);
        }

        public void DeactivateAttack()
        {
            if (attackHitbox != null) attackHitbox.SetActive(false);
        }

        // デバッグ用：攻撃範囲をシーンビューに表示
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