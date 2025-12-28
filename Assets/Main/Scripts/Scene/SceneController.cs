using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Main.Scene
{
    /// <summary>
    /// ゲーム内のシーン名を定義するenum
    /// </summary>
    public enum SceneName
    {
        Title,
        Stage1_1,
        Stage1_2,
        Stage1_3,
        Stage1_4,
        StageSelect,
        GameOver,
        Clear,
    }

    /// <summary>
    /// シーン遷移を管理するSingletonクラス
    /// Titleシーンで一度生成されれば、他のシーンでも利用可能
    /// </summary>
    public class SceneController : Singleton<SceneController>
    {
        protected override bool UseDontDestroyOnLoad => true;

        /// <summary>
        /// 現在のステージ情報
        /// </summary>
        public SceneName CurrentStage { get; private set; } = SceneName.Title;

        /// <summary>
        /// GameOver時にリトライで戻る対象のステージ
        /// </summary>
        public SceneName LastPlayableStage { get; private set; } = SceneName.Stage1_1;

        /// <summary>
        /// enumで指定されたシーンに切り替え
        /// </summary>
        /// <param name="sceneName">遷移先のシーン</param>
        public void LoadScene(SceneName sceneName)
        {
            // GameOverに入る直前の「プレイ中ステージ」を保持
            if (sceneName == SceneName.GameOver)
            {
                var stage = CurrentStage;
                if (TryGetActiveSceneName(out var activeSceneName))
                {
                    stage = activeSceneName;
                }

                if (IsPlayableStage(stage))
                {
                    LastPlayableStage = stage;
                }
            }

            CurrentStage = sceneName;

            // ステージに遷移したら、そのステージをリトライ対象として更新
            if (IsPlayableStage(sceneName))
            {
                LastPlayableStage = sceneName;
            }

            string sceneNameStr = sceneName.ToString();
            SceneManager.LoadScene(sceneNameStr);
        }

        /// <summary>
        /// GameOver画面から、直前に遊んでいたステージへ戻る
        /// </summary>
        public void RetryLastStage()
        {
            var retryStage = LastPlayableStage;
            if (!IsPlayableStage(retryStage))
            {
                retryStage = SceneName.Stage1_1;
            }

            LoadScene(retryStage);
        }

        /// <summary>
        /// 次のステージに進む
        /// </summary>
        public void LoadNextStage()
        {
            var stage = CurrentStage;
            if (TryGetActiveSceneName(out var activeSceneName))
            {
                stage = activeSceneName;
            }

            SceneName nextStage = GetNextStage(stage);
            LoadScene(nextStage);
        }

        /// <summary>
        /// 指定したステージの次のステージを取得
        /// </summary>
        /// <param name="currentStage">現在のステージ</param>
        /// <returns>次のステージ、最後の場合はClear</returns>
        private static SceneName GetNextStage(SceneName currentStage)
        {
            switch (currentStage)
            {
                case SceneName.Stage1_1:
                    return SceneName.Stage1_2;
                case SceneName.Stage1_2:
                    return SceneName.Stage1_3;
                case SceneName.Stage1_3:
                    return SceneName.Stage1_4;
                case SceneName.Stage1_4:
                    return SceneName.Clear;
                default:
                    return SceneName.Clear;
            }
        }

        private static bool TryGetActiveSceneName(out SceneName sceneName)
        {
            string active = SceneManager.GetActiveScene().name;
            return Enum.TryParse(active, out sceneName);
        }

        private static bool IsPlayableStage(SceneName sceneName)
        {
            // 現状は Stage1_1〜Stage1_4 がプレイ対象
            return sceneName == SceneName.Stage1_1
                || sceneName == SceneName.Stage1_2
                || sceneName == SceneName.Stage1_3
                || sceneName == SceneName.Stage1_4;
        }
    }
}
