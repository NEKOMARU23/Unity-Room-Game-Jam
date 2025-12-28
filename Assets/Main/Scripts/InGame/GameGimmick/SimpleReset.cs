using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理用
using UnityEngine.InputSystem;    // Input System用

public class SimpleReset : MonoBehaviour
{
    void Update()
    {
        // Rキーが押された瞬間だけ判定
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            // 今開いているシーンを最初からロードし直す
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}