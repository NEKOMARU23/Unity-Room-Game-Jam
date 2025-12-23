using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Main.InGame.Core
{
    public class monochromeChange : MonoBehaviour
    {
        [SerializeField] private Volume volume;
        [SerializeField] private ChannelMixer channelMixer;
        private void Start()
        {
            if (volume == null) Debug.LogError("グローバルボリュームが設定されていません");
            volume.profile.TryGet(out channelMixer);
        }
        private void EnableMono()
        {
            channelMixer.active = true;
        }

        private void DisableMono()
        {
            channelMixer.active = false;
        }
    }

}
