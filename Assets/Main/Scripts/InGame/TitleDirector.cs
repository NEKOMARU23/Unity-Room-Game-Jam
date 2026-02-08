using UnityEngine;

using MainS.Audio;

namespace Main.InGame
{
    /// <summary>
    /// タイトル画面の演出を管理するディレクタークラス。   
    public class TitleDirector : MonoBehaviour
    {
        void Start()
        {
            AudioManager.Instance.PlayBGM("Title");
        }
    }
}