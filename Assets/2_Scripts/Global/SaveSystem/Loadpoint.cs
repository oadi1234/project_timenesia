using System;
using _2___Scripts.Player;
using UnityEngine;

public class Loadpoint : MonoBehaviour
{
    public static event Action OnLoad;

    [SerializeField] private string _loadpointCoordinates;
    public string LoadpointCoordinates => _loadpointCoordinates;



    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnLoad != null && collision.CompareTag("Player"))
        {   
            var playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            OnLoad();
        }
    }
}
