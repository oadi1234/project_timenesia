using System;
using UnityEngine;

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
