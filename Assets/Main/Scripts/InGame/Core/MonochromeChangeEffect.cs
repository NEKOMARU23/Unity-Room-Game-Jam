using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class MonochromeChangeEffect : MonoBehaviour
{
    [SerializeField] private Material rippleMat;
    [SerializeField] public GameObject rippleScreen;
    private float rippleTime;
    private float disolveWaitSeconds = 1f;

    private bool lastRippleActive;
    private Coroutine disolveCoroutine;

    private void Awake()
    {
        if (rippleScreen != null) rippleScreen.SetActive(false);
        lastRippleActive = false;
    }

    private void Update()
    {
        // rippleScreen が有効化された瞬間に開始する
        bool rippleActive = rippleScreen != null && rippleScreen.activeInHierarchy;
        if (rippleActive && !lastRippleActive)
        {
            BeginRipple();
        }
        lastRippleActive = rippleActive;

        rippleTime += Time.deltaTime;
        rippleMat.SetFloat("RippleTime", rippleTime);
    }

    private void BeginRipple()
    {
        rippleTime = 0f;


        rippleMat.SetFloat("RippleTime", rippleTime);

        if (disolveCoroutine != null)
        {
            StopCoroutine(disolveCoroutine);
            disolveCoroutine = null;
        }

        disolveCoroutine = StartCoroutine(DisolveEffect());
    }

    private IEnumerator DisolveEffect()
    {
        yield return new WaitForSeconds(disolveWaitSeconds);
        if (rippleScreen != null) rippleScreen.SetActive(false);
        disolveCoroutine = null;
    }
}
