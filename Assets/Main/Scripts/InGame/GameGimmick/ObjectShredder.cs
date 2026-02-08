using UnityEngine;

namespace Main.InGame.GameGimmick
{
    /// <summary>
    /// 接触したオブジェクトを消去するギミック。
    /// 特定のタグを持つオブジェクト、またはすべてのオブジェクトを対象に削除を実行する。
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ObjectShredder : MonoBehaviour
    {
        [SerializeField] private string targetTag = "Enemy";

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsShreddable(collision.gameObject)) return;

            Shred(collision.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsShreddable(collision.gameObject)) return;

            Shred(collision.gameObject);
        }

        /// <summary>
        /// 対象のオブジェクトが削除可能（タグ条件に合致）かどうかを判定する
        /// </summary>
        private bool IsShreddable(GameObject target)
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                return true;
            }

            return target.CompareTag(targetTag);
        }

        /// <summary>
        /// オブジェクトの削除を実行する
        /// </summary>
        private void Shred(GameObject target)
        {
            Destroy(target);
        }
    }
}