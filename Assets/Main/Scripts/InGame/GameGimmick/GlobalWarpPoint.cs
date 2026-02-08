using UnityEngine;

namespace Main.InGame.GameGimmick
{
    /// <summary>
    /// 全インスタンスで共通のクールタイムを持つワープポイントを制御するクラス
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class GlobalWarpPoint : MonoBehaviour
    {
        [Header("ワープ設定")]
        [SerializeField] private Transform targetLocation;
        [SerializeField] private float globalCooldownTime = 2.0f;

        [Header("演出設定")]
        [SerializeField] private bool maintainVelocity = false;
        [SerializeField] private ParticleSystem warpEffect;

        private const string PLAYER_TAG = "Player";

        private static float nextWarpAllowedTime = 0f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(PLAYER_TAG)) return;
            if (Time.time < nextWarpAllowedTime) return;

            PerformWarp(other.gameObject);
        }

        /// <summary>
        /// プレイヤーを目標地点へ転送し、クールタイムを更新する
        /// </summary>
        private void PerformWarp(GameObject player)
        {
            if (targetLocation == null) return;

            UpdateGlobalCooldown();
            ApplyWarpPosition(player);
            ApplyPhysicsCorrection(player);
            PlayWarpEffect();
        }

        /// <summary>
        /// 全ワープポイント共通のクールタイムを更新する
        /// </summary>
        private void UpdateGlobalCooldown()
        {
            nextWarpAllowedTime = Time.time + globalCooldownTime;
        }

        /// <summary>
        /// プレイヤーの座標を目標地点へ移動させる
        /// </summary>
        private void ApplyWarpPosition(GameObject player)
        {
            player.transform.position = targetLocation.position;
        }

        /// <summary>
        /// 設定に基づきプレイヤーの物理速度を補正する
        /// </summary>
        private void ApplyPhysicsCorrection(GameObject player)
        {
            if (maintainVelocity) return;

            if (player.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        /// <summary>
        /// ワープ先の地点でエフェクトを生成する
        /// </summary>
        private void PlayWarpEffect()
        {
            if (warpEffect == null) return;

            Instantiate(warpEffect, targetLocation.position, Quaternion.identity);
        }
    }
}