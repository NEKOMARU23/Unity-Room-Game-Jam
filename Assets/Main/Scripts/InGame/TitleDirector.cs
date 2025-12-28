using UnityEngine;
using MainS.Audio;

public class TitleDirector : MonoBehaviour
{
    void Start()
    {
        // シーンが始まったらタイトルBGMを再生
        AudioManager.Instance.PlayBGM("Title");
    }
}