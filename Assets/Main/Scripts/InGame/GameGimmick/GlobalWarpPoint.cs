using UnityEngine;
using System.Collections;

namespace Main.Gimmick
{
    [RequireComponent(typeof(Collider2D))]
    public class GlobalWarpPoint : MonoBehaviour
    {
        [Header("ワープ設定")]
        [SerializeField] private Transform targetLocation;
        [SerializeField] private float globalCooldownTime = 2.0f;

        [Header("演出設定")]
        [SerializeField] private bool maintainVelocity = false;
        [SerializeField] private ParticleSystem warpEffect;

        // ★ 全てのインスタンスで共有される「次のワープ可能時刻」
        private static float _nextWarpAllowedTime = 0f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 現在の時間が、許可された時刻を過ぎているかチェック
            if (other.CompareTag("Player") && Time.time >= _nextWarpAllowedTime)
            {
                PerformWarp(other.gameObject);
            }
        }

        private void PerformWarp(GameObject player)
        {
            if (targetLocation == null) return;

            // 1. 次にワープできる時間を「全ワープ共通」で更新
            _nextWarpAllowedTime = Time.time + globalCooldownTime;

            // 2. 位置の移動
            player.transform.position = targetLocation.position;

            // 3. 物理挙動の補正
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null && !maintainVelocity)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // 4. エフェクト
            if (warpEffect != null)
            {
                Instantiate(warpEffect, targetLocation.position, Quaternion.identity);
            }
        }
    }
}