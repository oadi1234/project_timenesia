using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAreaGate : MonoBehaviour
{
    [SerializeField] private string AreaToLoad;

    public static Action<string> LoadArea;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LoadArea!=null && collision.gameObject.CompareTag("Player"))
        {
            LoadArea(AreaToLoad);
        }
    }
}
