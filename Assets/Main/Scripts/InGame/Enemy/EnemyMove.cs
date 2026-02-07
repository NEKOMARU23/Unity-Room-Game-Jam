using UnityEngine;
using System.Collections;

namespace Main.Enemy
{
    /// <summary>
    /// エネミーの巡回移動を制御するクラス。
    /// 指定された距離を往復、または片道移動し、方向転換時の演出を管理する。
    /// </summary>
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
        [SerializeField] private Sprite rightSprite;
        [SerializeField] private Sprite leftSprite;
        [SerializeField] private Sprite turnSprite;
        [SerializeField] private float turnDelay = 0.2f;

        private Vector3 startPosition;
        private bool movingRight;
        private bool isStopped = false;
        private bool isTurning = false;
        private SpriteRenderer spriteRenderer;
        private WaitForSeconds turnDelayWait;

        private void Awake()
        {
            startPosition = transform.position;
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            movingRight = startMovingRight;
            
            InitializeCache();
            UpdateSprite();
        }

        private void Update()
        {
            if (isStopped) return;
            if (isTurning) return;

            HandlePatrol();
        }

        /// <summary>
        /// 再利用するオブジェクトのキャッシュ初期化
        /// </summary>
        private void InitializeCache()
        {
            turnDelayWait = new WaitForSeconds(turnDelay);
        }

        /// <summary>
        /// 巡回移動の実行と端への到達判定
        /// </summary>
        private void HandlePatrol()
        {
            float rightEdge = startPosition.x + moveDistance;
            float leftEdge = startPosition.x - moveDistance;
            float direction = movingRight ? 1f : -1f;

            transform.position += new Vector3(direction * moveSpeed * Time.deltaTime, 0, 0);

            CheckEdgeReached(leftEdge, rightEdge);
        }

        /// <summary>
        /// 移動範囲の端に到達したかどうかのチェック
        /// </summary>
        private void CheckEdgeReached(float leftEdge, float rightEdge)
        {
            if (movingRight && transform.position.x >= rightEdge)
            {
                transform.position = new Vector3(rightEdge, transform.position.y, transform.position.z);
                HandleReachEdge();
                return;
            }
            
            if (!movingRight && transform.position.x <= leftEdge)
            {
                transform.position = new Vector3(leftEdge, transform.position.y, transform.position.z);
                HandleReachEdge();
            }
        }

        /// <summary>
        /// 端に到達した際の振る舞い決定
        /// </summary>
        private void HandleReachEdge()
        {
            if (moveMode == MoveMode.OneWay)
            {
                isStopped = true;
                return;
            }

            StartCoroutine(TurnRoutine());
        }

        /// <summary>
        /// 方向転換時の待機演出コルーチン
        /// </summary>
        private IEnumerator TurnRoutine()
        {
            isTurning = true;

            if (turnSprite != null)
            {
                spriteRenderer.sprite = turnSprite;
            }

            yield return turnDelayWait;

            movingRight = !movingRight;
            UpdateSprite();

            isTurning = false;
        }

        /// <summary>
        /// 現在の進行方向に基づいたスプライトの更新
        /// </summary>
        private void UpdateSprite()
        {
            if (spriteRenderer == null) return;

            if (movingRight)
            {
                ApplySprite(rightSprite);
                return;
            }

            ApplySprite(leftSprite);
        }

        /// <summary>
        /// スプライトの適用
        /// </summary>
        private void ApplySprite(Sprite targetSprite)
        {
            if (targetSprite != null)
            {
                spriteRenderer.sprite = targetSprite;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 center = Application.isPlaying ? startPosition : transform.position;
            Gizmos.color = Color.yellow;
            
            Vector3 leftPoint = center + Vector3.left * moveDistance;
            Vector3 rightPoint = center + Vector3.right * moveDistance;
            Vector3 markerSize = new Vector3(0.1f, 0.5f, 0);

            Gizmos.DrawLine(leftPoint, rightPoint);
            Gizmos.DrawWireCube(leftPoint, markerSize);
            Gizmos.DrawWireCube(rightPoint, markerSize);
        }
    }
}