using System;
using TMPro;
using UnityEngine;

namespace _2_Scripts.Global.SaveSystem
{
    public class Savepoint : MonoBehaviour
    {
        public static event Action<Vector2, ZoneEnum, string> OnSave;

        private Vector2 loadToLocationCoordinates;
        public ZoneEnum zone;
        private string sceneName;
        public TextController saveText;
        public bool canSave;

        private void Awake()
        {
            loadToLocationCoordinates = transform.position; //TODO make it maybe settable later on.
            sceneName = gameObject.scene.name;
        }

        private void Update()
        {
            if (OnSave != null && canSave && Input.GetButtonDown("Up"))
            {
                Debug.Log("Game saved!");
                //var playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                OnSave(loadToLocationCoordinates, zone, sceneName); //, playerHealth.currentHealth, gdm.stats.Coins);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                saveText.ShowText();
                canSave = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                saveText.HideText();
                canSave = false;
            }
        }
    }
}
