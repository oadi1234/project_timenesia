using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAttacher : MonoBehaviour
{
    private Canvas canvas;
    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = CameraScript.Instance.GetComponent<Camera>();
    }
}
