using System;
using UnityEngine;

namespace _2___Scripts.Global.Events
{
    internal class OnPlayerEnteredEvent : MonoBehaviour, IOnPlayerEnteredEvent
    {
        public static event Action<IOnPlayerEnteredEvent> OnPlayerEntered;

        [SerializeField] private string _eventName;
        [SerializeField] private int _numericData;
        public string EventName => _eventName;
        public int NumericData => _numericData;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (OnPlayerEntered != null && collision.CompareTag("Player"))
                OnPlayerEntered(this);
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

    }
}
