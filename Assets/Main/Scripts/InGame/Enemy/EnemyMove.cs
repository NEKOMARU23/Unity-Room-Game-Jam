using UnityEngine;

namespace Main.Enemy
{
    public class EnemyMove : MonoBehaviour
    {
        [Header("移動設定")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float moveDistance = 3f; // 中心から左右に動く距離
        
        private Vector3 startPosition;
        private bool movingRight = true;

        void Awake()
        {
            startPosition = transform.position;
        }

        void Update()
        {
            HandlePatrol();
        }

        private void HandlePatrol()
        {
            float rightEdge = startPosition.x + moveDistance;
            float leftEdge = startPosition.x - moveDistance;

            // --- 修正箇所：transform.Translate を使わず、直接座標を計算する ---
            float direction = movingRight ? 1f : -1f;
            transform.position += new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);

            // 右端に到達、または超えた場合
            if (movingRight && transform.position.x >= rightEdge)
            {
                // 位置を端に固定（突き抜け防止）
                transform.position = new Vector3(rightEdge, transform.position.y, transform.position.z);
                Flip();
            }
            // 左端に到達、または超えた場合
            else if (!movingRight && transform.position.x <= leftEdge)
            {
                // 位置を端に固定（突き抜け防止）
                transform.position = new Vector3(leftEdge, transform.position.y, transform.position.z);
                Flip();
            }
        }

        private void Flip()
        {
            movingRight = !movingRight;
            
            // 向きだけを変える
            if (movingRight) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            else transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        private void OnDrawGizmosSelected()
        {
            // 実行中は startPosition、編集時は現在の位置を基準に表示
            Vector3 center = Application.isPlaying ? startPosition : transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(center + Vector3.left * moveDistance, center + Vector3.right * moveDistance);
        }
    }
}