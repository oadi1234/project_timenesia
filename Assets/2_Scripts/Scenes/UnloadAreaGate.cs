using System;
using UnityEngine;

namespace _2_Scripts.Scenes
{
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
}
