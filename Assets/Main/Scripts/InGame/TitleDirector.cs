using UnityEngine;
using Main.Audio;

namespace Main.InGame
{
    /// <summary>
    /// タイトル画面の演出を管理するディレクタークラス。
    /// </summary>
    public class TitleDirector : MonoBehaviour
    {
        [Header("BGM設定")]
        [SerializeField] private string bgmName = "hidamari";
        [SerializeField] private float startTime = 0f;
        [SerializeField] private float loopDuration = 85f;

        void Start()
        {
            AudioManager.Instance.PlayBGM(bgmName, startTime, loopDuration);
        }
    }
}