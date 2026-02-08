using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace Main.InGame
{
    /// <summary>
    /// ゴール地点のトリガー処理を管理するクラス
    /// </summary>
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

            yield return new WaitForSeconds(waitSeconds);

            if (Scene.SceneController.Instance != null)
            {
                Scene.SceneController.Instance.LoadNextStage();
            }
            else
            {
                string activeScene = SceneManager.GetActiveScene().name;
                string nextScene = GetNextStageSceneName(activeScene);
                SceneManager.LoadScene(nextScene);
            }
        }

        private static string GetNextStageSceneName(string activeSceneName)
        {
            if (!Enum.TryParse(activeSceneName, out Scene.SceneName current))
            {
                return Scene.SceneName.Clear.ToString();
            }

            switch (current)
            {
                case Scene.SceneName.Stage1_1:
                    return Scene.SceneName.Stage1_2.ToString();
                case Scene.SceneName.Stage1_2:
                    return Scene.SceneName.Stage1_3.ToString();
                case Scene.SceneName.Stage1_3:
                    return Scene.SceneName.Stage1_4.ToString();
                case Scene.SceneName.Stage1_4:
                    return Scene.SceneName.Clear.ToString();
                default:
                    return Scene.SceneName.Clear.ToString();
            }
        }
    }
}