using UnityEngine;
using System.Collections;

namespace Main.Enemy
{
    public class EnemyMove : MonoBehaviour
    {
        public enum MoveMode { PingPong, OneWay }

        [Header("移動モード")]
        [SerializeField] private MoveMode moveMode = MoveMode.PingPong;

        [Header("移動設定")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float moveDistance = 3f;
        [SerializeField] private bool startMovingRight = true;

        [Header("スプライト設定")]
        [SerializeField] private Sprite rightSprite;  // 右移動中の画像
        [SerializeField] private Sprite leftSprite;   // 左移動中の画像
        [SerializeField] private Sprite turnSprite;   // 方向転換時の画像
        [SerializeField] private float turnDelay = 0.2f; // 方向転換画像を表示する時間

        private Vector3 startPosition;
        private bool movingRight;
        private bool isStopped = false;
        private bool isTurning = false; // 方向転換中フラグ
        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            startPosition = transform.position;
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            movingRight = startMovingRight;
            UpdateSprite(); // 初期画像の設定
        }

        void Update()
        {
            // 停止中、または方向転換中のアニメーション中は移動しない
            if (isStopped || isTurning) return; 

            HandlePatrol();
        }

        private void HandlePatrol()
        {
            float rightEdge = startPosition.x + moveDistance;
            float leftEdge = startPosition.x - moveDistance;

            float direction = movingRight ? 1f : -1f;
            transform.position += new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);

            if (movingRight && transform.position.x >= rightEdge)
            {
                transform.position = new Vector3(rightEdge, transform.position.y, transform.position.z);
                HandleReachEdge();
            }
            else if (!movingRight && transform.position.x <= leftEdge)
            {
                transform.position = new Vector3(leftEdge, transform.position.y, transform.position.z);
                HandleReachEdge();
            }
        }

        private void HandleReachEdge()
        {
            if (moveMode == MoveMode.PingPong)
            {
                // 方向転換コルーチンを開始
                StartCoroutine(TurnRoutine());
            }
            else
            {
                isStopped = true;
            }
        }

        // 方向転換の演出
        private IEnumerator TurnRoutine()
        {
            isTurning = true;

            // 1. 方向転換用の画像を表示
            if (turnSprite != null) spriteRenderer.sprite = turnSprite;

            // 2. 指定時間待つ（「おっとっと」という感じのタメ）
            yield return new WaitForSeconds(turnDelay);

            // 3. 向きを反転させて、通常移動の画像に戻す
            movingRight = !movingRight;
            UpdateSprite();

            isTurning = false;
        }

        // 移動方向に合わせた画像更新
        private void UpdateSprite()
        {
            if (spriteRenderer == null) return;

            if (movingRight)
            {
                if (rightSprite != null) spriteRenderer.sprite = rightSprite;
            }
            else
            {
                if (leftSprite != null) spriteRenderer.sprite = leftSprite;
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