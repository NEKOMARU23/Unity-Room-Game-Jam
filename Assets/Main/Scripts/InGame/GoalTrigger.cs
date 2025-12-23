using UnityEngine;
using System.Collections;
// 念のため、SceneManagerを直接使うための記述も追加しておきます
using UnityEngine.SceneManagement;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private string goalSeName = "GoalSE";
    [SerializeField] private float waitSeconds = 0.2f;
    private bool cleared = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (cleared) return;
        if (!other.CompareTag("Player")) return;

        cleared = true;
        StartCoroutine(PlaySeAndLoad());
    }

    private IEnumerator PlaySeAndLoad()
    {
        // SE再生待ち（必要ならコメントを外す）
        // if (AudioManager.Instance != null) AudioManager.Instance.Play(goalSeName);

        yield return new WaitForSeconds(waitSeconds);

        // --- 修正箇所：フルネームで指定 ---
        // Main.Scene という名前空間の中にある SceneController を探します
        if (Main.Scene.SceneController.Instance != null)
        {
            // SceneName.Clear も同様に名前空間付きで指定
            Main.Scene.SceneController.Instance.LoadScene(Main.Scene.SceneName.Clear);
        }
        else
        {
            // もしSceneControllerが見つからない場合のバックアップ（直接読み込み）
            // "Clear" という名前のシーンをビルド設定に入れている場合に動きます
            Debug.LogWarning("SceneControllerが見つからないため、直接シーンを読み込みます");
            SceneManager.LoadScene("Clear"); 
        }
        
        Debug.Log("ゴールしました！");
    }
}