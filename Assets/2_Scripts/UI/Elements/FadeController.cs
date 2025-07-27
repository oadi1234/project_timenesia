using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Elements
{
    public class FadeController : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        public Image fadeColour; //in case we want to edit colour to which fade is changed. Might be cool to have.
        public float fadeoutTime = 0.25f;
        private float elapsedTime;
        public float targetAlphaMin = 0f;
        public float targetAlphaMax = 1f;
        public float alphaAtStart = 0f;
        
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            fadeColour = GetComponent<Image>();
            elapsedTime = Mathf.Clamp(targetAlphaMax - alphaAtStart, targetAlphaMin, targetAlphaMax) * fadeoutTime;
            canvasGroup.alpha = alphaAtStart;
        }

        public IEnumerator DoFadeOut()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            while (canvasGroup.alpha > targetAlphaMin)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(1.0f - (elapsedTime / fadeoutTime));
                yield return null;
            }
        }

        public IEnumerator DoFadeIn()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            while (canvasGroup.alpha < targetAlphaMax)
            {
                elapsedTime -= Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(1.0f - (elapsedTime / fadeoutTime));
                yield return null;
            }
        }

        public void Reset()
        {
            elapsedTime = Mathf.Clamp(targetAlphaMax - alphaAtStart, targetAlphaMin, targetAlphaMax) * fadeoutTime;
            canvasGroup.alpha = targetAlphaMin;
        }

        public void InstantSnapToMinAlpha()
        {
            if (canvasGroup)
            {
                canvasGroup.alpha = Mathf.Clamp01(targetAlphaMin);
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            elapsedTime = Mathf.Clamp01(targetAlphaMin) * fadeoutTime;

        }
        
        public void InstantSnapToMaxAlpha()
        {
            if (canvasGroup)
            {
                canvasGroup.alpha = Mathf.Clamp01(targetAlphaMax);
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            elapsedTime = Mathf.Clamp01(targetAlphaMax) * fadeoutTime;

        }
    }
}
