using System;
using _2___Scripts.Global;
using _2___Scripts.Player;
using UnityEngine;

public class Savepoint : MonoBehaviour
{
    public GameDataManager gdm;
    public static event Action<string, int, int> OnSave;

    [SerializeField] private string _savepointCoordinates;
    public string SavepointCoordinates => _savepointCoordinates;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnSave != null && collision.CompareTag("Player"))
        {   
            var playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            OnSave(_savepointCoordinates, playerHealth.currentHealth, gdm.Coins);
        }
    }
}
