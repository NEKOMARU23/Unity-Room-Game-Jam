using UnityEngine;
using System.Collections.Generic;

namespace Main.Gimmick
{
    public class PhysicalSwitchFloater : MonoBehaviour
    {
        [Header("浮かせたい対象")]
        [SerializeField] private Transform targetObject;

        [Header("浮遊設定")]
        [SerializeField] private Vector3 floatOffset = new Vector3(0, 2f, 0); 
        [SerializeField] private float smoothSpeed = 5f;

        [Header("スイッチの設定")]
        [SerializeField] private bool debugMode = false;

        private Vector3 _startPosition;
        private Vector3 _goalPosition;
        
        private HashSet<Collider2D> _onSwitchObjects = new HashSet<Collider2D>();

        // レイヤー名をあらかじめ取得（マジックナンバーを避けるため）
        private int _recordedPlayerLayer;

        private void Start()
        {
            if (targetObject != null)
            {
                _startPosition = targetObject.position;
                _goalPosition = _startPosition + floatOffset;
            }

            // ゴースト用のレイヤー番号を取得
            _recordedPlayerLayer = LayerMask.NameToLayer("Player_Recorded");
        }

        private void Update()
        {
            if (targetObject == null) return;

            bool isPressed = _onSwitchObjects.Count > 0;
            Vector3 targetPos = isPressed ? _goalPosition : _startPosition;
            targetObject.position = Vector3.Lerp(targetObject.position, targetPos, Time.deltaTime * smoothSpeed);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 1. 本物のプレイヤー (Tag: Player)
            // 2. ゴーストの自分 (Layer: Player_Recorded)
            // 3. その他重し (Layer: Default 等)
            if (other.CompareTag("Player") || 
                other.gameObject.layer == _recordedPlayerLayer ||
                other.gameObject.layer == LayerMask.NameToLayer("Default")) 
            {
                _onSwitchObjects.Add(other);
                if (debugMode) Debug.Log($"[Switch ON] {other.name} が乗りました（Layer: {LayerMask.LayerToName(other.gameObject.layer)}）");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_onSwitchObjects.Contains(other))
            {
                _onSwitchObjects.Remove(other);
                if (debugMode) Debug.Log($"[Switch OFF] {other.name} が離れました");
            }
        }
    }
}