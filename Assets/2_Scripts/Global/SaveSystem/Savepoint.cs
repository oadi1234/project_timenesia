using System;
using UnityEngine;

namespace _2_Scripts.Global.SaveSystem
{
    public class Savepoint : MonoBehaviour
    {
        public GameDataManager gdm;
        public static event Action<Vector2, ZoneEnum, string> OnSave;

        private Vector2 loadToLocationCoordinates;
        public ZoneEnum zone;
        private string sceneName;

        private void Awake()
        {
            gdm = GameDataManager.Instance;
            loadToLocationCoordinates = transform.position; //TODO make it maybe settable later on.
            sceneName = gameObject.scene.name;
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
}
