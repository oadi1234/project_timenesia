using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadAreaGate : MonoBehaviour
{
    [SerializeField] private string AreaToUnload;

    public static Action<string> UnloadArea;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (UnloadArea != null && collision.gameObject.CompareTag("Player"))
        {
            UnloadArea(AreaToUnload);
        }
    }
}
