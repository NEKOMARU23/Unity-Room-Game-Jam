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
        [SerializeField] private ColorAdjustments colorAdjustments;
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
            volume.profile.TryGet(out colorAdjustments);
            isMonochrome = false;
        }
        public void EnableMono()
        {
            channelMixer.active = true;
            colorAdjustments.active = true;
            isMonochrome = true;
        }

        public void DisableMono()
        {
            channelMixer.active = false;
            colorAdjustments.active = false;
            isMonochrome = false;
        }
    }

}
