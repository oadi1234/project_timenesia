using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Elements
{
    public class FadeController : MonoBehaviour
    {
        private CanvasGroup overlay { get {  return GetComponent<CanvasGroup>(); } }
        public Image fadeColour { get { return GetComponent<Image>(); } set { fadeColour = value; } } //in case we want to edit colour to which fade is changed. Might be cool to have.
        public float fadeoutTime = 0.25f;
        private float elapsedTime;
        public float targetAlphaMin = 0f;
        public float targetAlphaMax = 1f;
        public float alphaAtStart = 0f;

        private void Awake()
        {
            elapsedTime = Mathf.Clamp(targetAlphaMax - alphaAtStart, targetAlphaMin, targetAlphaMax) * fadeoutTime;
            overlay.alpha = alphaAtStart;
        }

        public IEnumerator DoFadeOut()
        {
            while (overlay.alpha > targetAlphaMin)
            {
                elapsedTime += Time.deltaTime;
                overlay.alpha = Mathf.Clamp01(1.0f - (elapsedTime / fadeoutTime));
                yield return null;
            }
        }

        public IEnumerator DoFadeIn()
        {
            while (overlay.alpha < targetAlphaMax)
            {
                elapsedTime -= Time.deltaTime;
                overlay.alpha = Mathf.Clamp01(1.0f - (elapsedTime / fadeoutTime));
                yield return null;
            }
        }

        public void Reset()
        {
            elapsedTime = Mathf.Clamp(targetAlphaMax - alphaAtStart, targetAlphaMin, targetAlphaMax) * fadeoutTime;
            overlay.alpha = targetAlphaMin;
        }
    }
}
