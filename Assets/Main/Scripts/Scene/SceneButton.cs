using UnityEngine;

namespace Main.Scene
{
    /// <summary>
    /// シーン遷移ボタンのラッパークラス
    /// SceneControllerが他のシーンにない場合でも、enumを使ってシーン遷移が可能
    /// </summary>
    public class SceneButton : MonoBehaviour
    {
        [Header("シーン遷移設定")]
        [SerializeField] private SceneName targetScene = SceneName.Title;

        /// <summary>
        /// ボタンクリック時に呼び出される
        /// インスペクターで指定されたシーンに遷移
        /// </summary>
        public void OnClick()
        {
            // ボタンクリック音を再生
            //if (AudioManager.Instance != null)
            // AudioManager.Instance.Play("Button");

            if (SceneController.Instance != null)
                SceneController.Instance.LoadScene(targetScene);
            else
                Debug.LogError("SceneControllerのインスタンスが見つかりません！Titleシーンにアタッチされているか確認してください。");
        }

        /// <summary>
        /// プログラムからシーンを指定して遷移
        /// </summary>
        /// <param name="sceneName">遷移先シーン</param>
        public void LoadScene(SceneName sceneName)
        {
            if (SceneController.Instance != null)
                SceneController.Instance.LoadScene(sceneName);
            else
                Debug.LogError("SceneControllerのインスタンスが見つかりません！Titleシーンにアタッチされているか確認してください。");
        }
    }
}