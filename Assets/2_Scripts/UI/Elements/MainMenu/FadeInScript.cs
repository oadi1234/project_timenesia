using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInScript : MonoBehaviour
{
    public FadeController BlackPanel;

    // Update is called once per frame
    void Awake()
    {
        StartCoroutine(WaitSingleFrameThenFadeOut());
    }

    private IEnumerator WaitSingleFrameThenFadeOut()
    {
        yield return new WaitForEndOfFrame();
        yield return BlackPanel.DoFadeOut();
        Destroy(this);
    }
}
