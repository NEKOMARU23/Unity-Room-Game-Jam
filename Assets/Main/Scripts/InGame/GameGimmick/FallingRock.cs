using UnityEngine;
using System.Collections;

namespace Main.InGame.GameGimmick
{
    /// <summary>
    /// 一定間隔で出現・落下・消滅を繰り返す落石オブジェクトの挙動を制御する
    /// </summary>
    public class FallingRock : MonoBehaviour
    {
        [Header("サイクル設定")]
        [SerializeField] private float activeDuration = 5.0f;
        [SerializeField] private float respawnDelay = 2.0f;

        private Vector3 startPosition;
        private Quaternion startRotation;
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Collider2D rockCollider;

        private WaitForSeconds activeDurationWait;
        private WaitForSeconds respawnDelayWait;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            rockCollider = GetComponent<Collider2D>();

            startPosition = transform.position;
            startRotation = transform.rotation;

            InitializeCache();
        }

        private void Start()
        {
            StartCoroutine(RockCycle());
        }

        /// <summary>
        /// 再利用するオブジェクトのキャッシュ初期化
        /// </summary>
        private void InitializeCache()
        {
            activeDurationWait = new WaitForSeconds(activeDuration);
            respawnDelayWait = new WaitForSeconds(respawnDelay);
        }

        /// <summary>
        /// 出現、待機、消滅のループサイクルを管理する
        /// </summary>
        private IEnumerator RockCycle()
        {
            while (true)
            {
                ResetRockState();
                SetAppearance(true);

                yield return activeDurationWait;

                SetAppearance(false);

                yield return respawnDelayWait;
            }
        }

        /// <summary>
        /// 岩の位置、回転、物理速度を初期状態に戻す
        /// </summary>
        private void ResetRockState()
        {
            transform.position = startPosition;
            transform.rotation = startRotation;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        /// <summary>
        /// 描画、当たり判定、物理シミュレーションの有効状態を切り替える
        /// </summary>
        private void SetAppearance(bool isVisible)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = isVisible;
            }

            if (rockCollider != null)
            {
                rockCollider.enabled = isVisible;
            }

            if (rb != null)
            {
                rb.simulated = isVisible;
            }
        }
    }
}