using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class monochromeChange : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Volume targetVolume;

    [Header("Profiles (optional)")]
    [SerializeField] private VolumeProfile normalProfile;
    [SerializeField] private VolumeProfile monochromeProfile;

    private bool isMonochrome;

    private void Awake()
    {
        if (targetVolume == null)
        {
            targetVolume = GetComponent<Volume>();
        }

        if (targetVolume == null)
        {
            targetVolume = FindFirstObjectByType<Volume>();
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TogglePostEffect();
        }
    }

    public void TogglePostEffect()
    {
        if (targetVolume == null)
        {
            return;
        }

        isMonochrome = !isMonochrome;

        if (normalProfile != null && monochromeProfile != null)
        {
            targetVolume.profile = isMonochrome ? monochromeProfile : normalProfile;
            targetVolume.weight = 1f;
            targetVolume.enabled = true;
            return;
        }

        // プロファイル差し替えが無い場合は、このVolume自体をON/OFFして切り替える
        targetVolume.enabled = isMonochrome;
    }
}
