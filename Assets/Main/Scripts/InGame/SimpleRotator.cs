using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // 1秒間の回転角度

    void Update()
    {
        // 自機（Z軸）を中心に回転させる
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}