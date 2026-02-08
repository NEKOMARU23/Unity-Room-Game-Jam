using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Main.InGame.GameGimmick
{
    /// <summary>
    /// キー入力によるシーンのリセット機能を管理するクラス
    /// </summary>
    public class SimpleReset : MonoBehaviour
    {
        private Keyboard currentKeyboard;

        private void Awake()
        {
            InitializeCache();
        }

        private void Update()
        {
            HandleResetInput();
        }

        /// <summary>
        /// 使用するコンポーネントやデバイスの参照をキャッシュする
        /// </summary>
        private void InitializeCache()
        {
            currentKeyboard = Keyboard.current;
        }

        /// <summary>
        /// リセット入力の検知およびシーンの再読み込みを実行する
        /// </summary>
        private void HandleResetInput()
        {
            if (currentKeyboard == null) return;
            if (!currentKeyboard.rKey.wasPressedThisFrame) return;

            ResetCurrentScene();
        }

        /// <summary>
        /// 現在アクティブなシーンを再読み込みする
        /// </summary>
        private void ResetCurrentScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}