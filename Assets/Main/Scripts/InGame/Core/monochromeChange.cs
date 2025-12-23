using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Main.InGame.Core
{
    ///<summary>
    ///グローバルボリュームを取得しモノクロにする処理を書いたクラス
    ///<summary>
    public class MonochromeChange : MonoBehaviour
    {
        [SerializeField] private Volume volume;
        [SerializeField] private ChannelMixer channelMixer;
        [Tooltip("trueならモノクロ")]public bool isMonochrome;

        private void Awake()
        {
            volume ??= GetComponent<Volume>();

            if (volume == null)
            {
                Debug.LogError("Volume がアタッチされていません");
                return;
            }

            volume.profile.TryGet(out channelMixer);
            isMonochrome = false;
        }
        public void EnableMono()
        {
            channelMixer.active = true;
            isMonochrome = true;
        }

        public void DisableMono()
        {
            channelMixer.active = false;
            isMonochrome = false;
        }
    }

}
