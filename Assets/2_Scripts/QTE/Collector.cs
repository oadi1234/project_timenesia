using System;
using System.Collections;
using System.Collections.Generic;
using _2___Scripts.Global.Events;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class Collector : MonoBehaviour, IBaseEvent
{
    public static event Action<IBaseEvent> OnCollected;
    [SerializeField] public int counter = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        OnPlayerEnteredEvent.OnPlayerEntered += OnPlayer_Entered;
    }
    

    private void OnPlayer_Entered(IOnPlayerEnteredEvent obj)
    {
        if (obj.eventType.Equals("KeyCollected"))
        {
            obj.Remove();
            counter--;
            if (counter <= 0)
            {
                // Debug.Log("DONE");
                if (OnCollected != null)
                    OnCollected(this);
            }
        }
    }

    public string EventName => "KeysCollected";
}
