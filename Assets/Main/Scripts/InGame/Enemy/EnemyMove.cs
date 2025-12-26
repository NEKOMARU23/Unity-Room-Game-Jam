using UnityEngine;

namespace Main.Enemy
{
    public class EnemyMove : MonoBehaviour
    {
        // 移動パターンの定義
        public enum MoveMode { PingPong, OneWay }

        [Header("移動モード")]
        [SerializeField] private MoveMode moveMode = MoveMode.PingPong;

        [Header("移動設定")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float moveDistance = 3f; // 中心から左右に動く距離
        
        [SerializeField] private bool startMovingRight = true;
        [SerializeField] private bool useFlipX = true;

        private Vector3 startPosition;
        private bool movingRight;
        private bool isStopped = false; // 片道モードで停止したか
        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            startPosition = transform.position;
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            movingRight = startMovingRight;
            ApplyRotation();
        }

        void Update()
        {
            if (isStopped) return; // 停止していたら何もしない
            HandlePatrol();
        }

        private void HandlePatrol()
        {
            float rightEdge = startPosition.x + moveDistance;
            float leftEdge = startPosition.x - moveDistance;

            float direction = movingRight ? 1f : -1f;
            transform.position += new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);

            // 右端に到達
            if (movingRight && transform.position.x >= rightEdge)
            {
                transform.position = new Vector3(rightEdge, transform.position.y, transform.position.z);
                HandleReachEdge();
            }
            // 左端に到達
            else if (!movingRight && transform.position.x <= leftEdge)
            {
                transform.position = new Vector3(leftEdge, transform.position.y, transform.position.z);
                HandleReachEdge();
            }
        }

        // 端に到達した時の処理
        private void HandleReachEdge()
        {
            if (moveMode == MoveMode.PingPong)
            {
                // 往復モード：反転する
                movingRight = !movingRight;
                ApplyRotation();
            }
            else
            {
                // 片道モード：停止する
                isStopped = true;
            }
        }

        private void ApplyRotation()
        {
            if (useFlipX && spriteRenderer != null)
            {
                spriteRenderer.flipX = !movingRight;
            }
            else
            {
                if (movingRight) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                else transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 center = Application.isPlaying ? startPosition : transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(center + Vector3.left * moveDistance, center + Vector3.right * moveDistance);
            
            Gizmos.DrawWireCube(center + Vector3.left * moveDistance, new Vector3(0.1f, 0.5f, 0));
            Gizmos.DrawWireCube(center + Vector3.right * moveDistance, new Vector3(0.1f, 0.5f, 0));
        }
    }
}