using UnityEngine;

namespace Main.InGame
{
    /// <summary>
    /// オブジェクトを一定速度で回転させるシンプルなコンポーネント。
    /// </summary>
    public class SimpleRotator : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 100f; // 1秒間の回転角度

        void Update()
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}