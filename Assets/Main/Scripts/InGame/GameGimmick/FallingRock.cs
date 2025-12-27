using UnityEngine;
using System.Collections;

namespace Main.Gimmick
{
    public class FallingRock : MonoBehaviour
    {
        [Header("サイクル設定")]
        [SerializeField] private float activeDuration = 5.0f;  // 出現してから消えるまでの時間
        [SerializeField] private float respawnDelay = 2.0f;   // 消えてから再出現するまでの待ち時間

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Rigidbody2D _rb;
        private SpriteRenderer _renderer;
        private Collider2D _collider;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();

            // 最初の位置と回転を記録
            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        private void Start()
        {
            // 最初のサイクルを開始
            StartCoroutine(RockCycle());
        }

        private IEnumerator RockCycle()
        {
            while (true)
            {
                // 1. 出現（初期位置に戻して物理をリセット）
                ResetRock();
                SetAppearance(true);

                // 2. 一定時間待機（落下している時間）
                yield return new WaitForSeconds(activeDuration);

                // 3. 消滅
                SetAppearance(false);

                // 4. 再出現までの待ち時間
                yield return new WaitForSeconds(respawnDelay);
            }
        }

        private void ResetRock()
        {
            // 位置と回転を戻す
            transform.position = _startPosition;
            transform.rotation = _startRotation;

            // 物理挙動（速度や回転）を完全にゼロにする
            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0f;
            }
        }

        private void SetAppearance(bool isVisible)
        {
            if (_renderer != null) _renderer.enabled = isVisible;
            if (_collider != null) _collider.enabled = isVisible;
            
            // 物理演算を止める/動かす
            if (_rb != null)
            {
                _rb.simulated = isVisible;
            }
        }
    }
}