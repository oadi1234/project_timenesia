using System;
using _2_Scripts.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2_Scripts.Global.Events
{
    internal class OnPlayerEnteredEvent : MonoBehaviour, IOnPlayerEnteredEvent
    {
        public static event Action<IOnPlayerEnteredEvent> OnPlayerEntered;

        [SerializeField] private CollectedEventType _collectedEventType;
        [SerializeField] private int _numericData;
        public CollectedEventType collectedEventType => _collectedEventType;
        public int numericData => _numericData;
        public string sceneName => gameObject.scene.name;
        public string objectName => gameObject.name;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == (int) Layers.Player)
                OnPlayerEntered?.Invoke(this);
        }

        public void Remove()
        {
            Destroy(gameObject);
        }
    }
}
