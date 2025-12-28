using UnityEngine;
using System.Collections;
// 念のため、SceneManagerを直接使うための記述も追加しておきます
using UnityEngine.SceneManagement;
using System;

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

        if (Main.Scene.SceneController.Instance != null)
        {
            Main.Scene.SceneController.Instance.LoadNextStage();
        }
        else
        {
            Debug.LogWarning("SceneControllerが見つからないため、直接シーンを読み込みます");

            string activeScene = SceneManager.GetActiveScene().name;
            string nextScene = GetNextStageSceneName(activeScene);
            SceneManager.LoadScene(nextScene);
        }
        
        Debug.Log("ゴールしました！");
    }

    private static string GetNextStageSceneName(string activeSceneName)
    {
        if (!Enum.TryParse(activeSceneName, out Main.Scene.SceneName current))
        {
            return Main.Scene.SceneName.Clear.ToString();
        }

        switch (current)
        {
            case Main.Scene.SceneName.Stage1_1:
                return Main.Scene.SceneName.Stage1_2.ToString();
            case Main.Scene.SceneName.Stage1_2:
                return Main.Scene.SceneName.Stage1_3.ToString();
            case Main.Scene.SceneName.Stage1_3:
                return Main.Scene.SceneName.Stage1_4.ToString();
            case Main.Scene.SceneName.Stage1_4:
                return Main.Scene.SceneName.Clear.ToString();
            default:
                return Main.Scene.SceneName.Clear.ToString();
        }
    }
}