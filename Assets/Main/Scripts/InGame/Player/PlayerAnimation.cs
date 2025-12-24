using UnityEngine;

namespace Main.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator anim;
        private PlayerMove playerMove;
        [SerializeField] private GameObject attackHitbox;

        void Awake()
        {
            anim = GetComponent<Animator>();
            playerMove = GetComponent<PlayerMove>();
        }

        void Update()
        {
            if (anim == null || playerMove == null) return;

            if (!playerMove.IsGrounded())
            {
                anim.ResetTrigger("Attack");
            }

            // 1. 横移動速度の反映
            float horizontalSpeed = Mathf.Abs(playerMove.GetCurrentMoveInput().x);
            anim.SetFloat("Speed", horizontalSpeed);

            // 2. 接地判定を Jump パラメーターに反映
            // 地面にいない（!IsGrounded）ときに Jump を true にする
            bool isInAir = !playerMove.IsGrounded();
            anim.SetBool("Jump", isInAir);
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

        public void ActivateAttack()
        {

            if (attackHitbox != null) attackHitbox.SetActive(true);
        }

        public void DeactivateAttack()
        {
            if (attackHitbox != null) attackHitbox.SetActive(false);
        }
    }
}