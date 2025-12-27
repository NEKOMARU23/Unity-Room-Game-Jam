using UnityEngine;

namespace Main.Gimmick
{
    public class PhysicalRotator : MonoBehaviour
    {
        [Header("ゴースト（再生）時の重み設定")]
        [SerializeField] private float ghostWeightForce = 10f; // ゴーストが乗った時にかける力

        private Rigidbody2D _rb;
        private int _recordedPlayerLayer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _recordedPlayerLayer = LayerMask.NameToLayer("Player_Recorded");
        }

        // ゴーストが上に乗っている間、継続的に力を加える
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == _recordedPlayerLayer)
            {
                // ゴーストの位置が中心より右か左かを判定
                float direction = other.transform.position.x > transform.position.x ? -1f : 1f;
                
                // 回転させる力（トルク）を加える
                _rb.AddTorque(direction * ghostWeightForce);
            }
        }
    }
}