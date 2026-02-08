using UnityEngine;
using System.Collections.Generic;

namespace Main.InGame.GameGimmick
{
    /// <summary>
    /// 物理的なスイッチの踏下状態を検知し、ターゲットオブジェクトを浮遊させる。
    /// プレイヤーおよび録画済みプレイヤーに反応する。
    /// </summary>
    public class PhysicalSwitchFloater : MonoBehaviour
    {
        [Header("浮かせたい対象")]
        [SerializeField] private Transform targetObject;

        [Header("浮遊設定")]
        [SerializeField] private Vector3 floatOffset = new Vector3(0, 2f, 0);
        [SerializeField] private float smoothSpeed = 5f;

        [Header("スイッチの設定")]
        [SerializeField] private bool debugMode = false;

        private const string PLAYER_TAG = "Player";
        private const string RECORDED_PLAYER_LAYER_NAME = "Player_Recorded";
        private const string DEFAULT_LAYER_NAME = "Default";

        private Vector3 startPosition;
        private Vector3 goalPosition;
        private HashSet<Collider2D> onSwitchObjects = new HashSet<Collider2D>();

        private int recordedPlayerLayer;
        private int defaultLayer;

        private void Awake()
        {
            InitializeTransformSettings();
            InitializeLayerCache();
        }

        private void Update()
        {
            if (targetObject == null) return;

            CleanInvalidColliders();
            UpdateMovement();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsEligibleObject(other)) return;

            onSwitchObjects.Add(other);

            if (debugMode)
            {
                Debug.Log($"[Switch ON] {other.name} (Layer: {LayerMask.LayerToName(other.gameObject.layer)})");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (onSwitchObjects.Remove(other))
            {
                if (debugMode)
                {
                    Debug.Log($"[Switch OFF] {other.name}");
                }
            }
        }

        /// <summary>
        /// 初期位置と目標位置の設定
        /// </summary>
        private void InitializeTransformSettings()
        {
            if (targetObject == null) return;

            startPosition = targetObject.position;
            goalPosition = startPosition + floatOffset;
        }

        /// <summary>
        /// レイヤーIDのキャッシュ
        /// </summary>
        private void InitializeLayerCache()
        {
            recordedPlayerLayer = LayerMask.NameToLayer(RECORDED_PLAYER_LAYER_NAME);
            defaultLayer = LayerMask.NameToLayer(DEFAULT_LAYER_NAME);
        }

        /// <summary>
        /// 参照が失われた、または無効になったオブジェクトの除去
        /// </summary>
        private void CleanInvalidColliders()
        {
            onSwitchObjects.RemoveWhere(col => col == null || !col.enabled || !col.gameObject.activeInHierarchy);
        }

        /// <summary>
        /// ターゲットオブジェクトの座標更新
        /// </summary>
        private void UpdateMovement()
        {
            bool isPressed = onSwitchObjects.Count > 0;
            Vector3 targetPos = isPressed ? goalPosition : startPosition;

            targetObject.position = Vector3.Lerp(
                targetObject.position,
                targetPos,
                Time.deltaTime * smoothSpeed
            );
        }

        /// <summary>
        /// 判定対象のオブジェクトかどうかの確認
        /// </summary>
        private bool IsEligibleObject(Collider2D other)
        {
            if (other.CompareTag(PLAYER_TAG)) return true;

            int layer = other.gameObject.layer;
            if (layer == recordedPlayerLayer && recordedPlayerLayer != -1) return true;
            if (layer == defaultLayer && defaultLayer != -1) return true;

            return false;
        }
    }
}