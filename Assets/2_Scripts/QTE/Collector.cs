using System;
using _2___Scripts.Global.Events;
using _2_Scripts.Global.Events;
using UnityEngine;

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
