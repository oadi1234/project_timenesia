using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    private CanvasGroup overlay { get {  return GetComponent<CanvasGroup>(); } }
    public Image fadeColour { get { return GetComponent<Image>(); } set { fadeColour = value; } } //in case we want to edit colour to which fade is changed. Might be cool to have.
    public float fadeoutTime = 0.25f;
    private float elapsedTime = 0.25f;
    public float targetAlphaMin = 0f;
    public float targetAlphaMax = 1f;

    private void Awake()
    {
        elapsedTime = fadeoutTime;
        overlay.alpha = targetAlphaMin;
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

    public void SetFadeTimer(float time = 0)
    {
        fadeoutTime = time;
    }
}
