using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    private CanvasGroup overlay { get {  return GetComponent<CanvasGroup>(); } }
    private Image fadeColour { get { return GetComponent<Image>(); } set { fadeColour = value; } } //in case we want to edit colour to which fade is changed. Might be cool to have.
    public float fadeoutTime = 0.25f;
    private float elapsedTime = 0.25f;

    private void Awake()
    {
        elapsedTime = fadeoutTime;
    }

    public IEnumerator DoFadeOut()
    {
        while (overlay.alpha > 0)
        {
            elapsedTime += Time.deltaTime;
            overlay.alpha = Mathf.Clamp01(1.0f - (elapsedTime / fadeoutTime));
            yield return null;
        }
    }

    public IEnumerator DoFadeIn()
    {
        while (overlay.alpha < 1f)
        {
            elapsedTime -= Time.deltaTime;
            overlay.alpha = Mathf.Clamp01(1.0f - (elapsedTime / fadeoutTime));
            yield return null;
        }
    }
}
