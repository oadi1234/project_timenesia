using System;
using _2___Scripts.Global;
using _2___Scripts.Player;
using UnityEngine;

public class Savepoint : MonoBehaviour
{
    public GameDataManager gdm;
    public static event Action<Vector2, ZoneEnum, string> OnSave;

    private Vector2 loadToLocationCoordinates;
    public ZoneEnum zone;
    public string sceneName;

    private void Awake()
    {
        gdm = GameDataManager.Instance;
        loadToLocationCoordinates = transform.position; //TODO make it maybe settable later on.
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnSave != null && collision.CompareTag("Player"))
        {
            //var playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            OnSave(loadToLocationCoordinates, zone, sceneName); //, playerHealth.currentHealth, gdm.stats.Coins);
        }
    }
}
