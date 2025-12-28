using UnityEngine;

public class ObjectShredder : MonoBehaviour
{
    // 特定のタグ（例：PlayerやEnemy）だけを消したい場合に設定
    [SerializeField] private string targetTag = "Enemy";


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // もし targetTag が空なら、触れたもの全てを消す
        // 特定のタグが設定されているなら、そのタグを持つものだけを消す
        if (string.IsNullOrEmpty(targetTag) || collision.CompareTag(targetTag))
        {
            Debug.Log($"{collision.gameObject.name} をデストロイしました");
            Destroy(collision.gameObject);
        }
    }

    // 物理衝突（Is Triggerオフ）で消したい場合はこちら
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (string.IsNullOrEmpty(targetTag) || collision.gameObject.CompareTag(targetTag))
        {
            Destroy(collision.gameObject);
        }
    }
}