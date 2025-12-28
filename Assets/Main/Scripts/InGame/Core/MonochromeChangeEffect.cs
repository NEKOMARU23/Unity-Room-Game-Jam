using System.Collections;
using UnityEngine;

namespace Main.InGame.Core
{
    public class MonochromeChangeEffect : MonoBehaviour
    {
        [SerializeField] private Material rippleMat;
        private float rippleTime;
        private float disolveWaitSeconds = 1f;

        private void Awake()
        {
            gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            rippleTime = 0f;
            rippleMat.SetFloat("_RippleTime", rippleTime);
            StartCoroutine("DisolveRipple");
        }

        private void Update()
        {
            rippleTime += Time.deltaTime;
            rippleMat.SetFloat("_RippleTime", rippleTime);
        }

        private IEnumerator DisolveRipple()
        {
            yield return new WaitForSeconds(disolveWaitSeconds);
            gameObject.SetActive(false);
        }
    }
}
