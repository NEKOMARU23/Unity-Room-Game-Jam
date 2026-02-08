using UnityEngine;

namespace Main.InGame.GameGimmick
{
    /// <summary>
    /// 特定のレイヤーを持つオブジェクトが接触している間、その位置に応じてトルクを加えるクラス
    /// </summary>
    public class PhysicalRotator : MonoBehaviour
    {
        [Header("ゴースト（再生）時の重み設定")]
        [SerializeField] private float ghostWeightForce = 10f;

        private Rigidbody2D rb;
        private int recordedPlayerLayer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            recordedPlayerLayer = LayerMask.NameToLayer("Player_Recorded");
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!IsRecordedPlayer(other.gameObject.layer)) return;

            ApplyTorqueBasedOnPosition(other.transform.position);
        }

        /// <summary>
        /// 対象のレイヤーが録画済みプレイヤー（ゴースト）のものか判定する
        /// </summary>
        private bool IsRecordedPlayer(int layer)
        {
            return layer == recordedPlayerLayer;
        }

        /// <summary>
        /// オブジェクトの相対位置に基づいて回転力を加える
        /// </summary>
        private void ApplyTorqueBasedOnPosition(Vector3 otherPosition)
        {
            // ルール10: 三項演算子で計算負荷を抑えつつ方向を決定
            float direction = otherPosition.x > transform.position.x ? -1f : 1f;
            
            rb.AddTorque(direction * ghostWeightForce);
        }
    }
}