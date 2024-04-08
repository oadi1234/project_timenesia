using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualFader : MonoBehaviour
{
    public FadeController fadeController;
    public float delay = 0.1f;
    public int sequenceIndex = 0;

    private void OnEnable()
    {
        fadeController.StopAllCoroutines();
        StopAllCoroutines();
        StartCoroutine(Activate());
    }

    private IEnumerator Activate()
    {
        yield return new WaitForSeconds(sequenceIndex * delay);
        StartCoroutine(fadeController.DoFadeIn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        fadeController.Reset();
    }
}
