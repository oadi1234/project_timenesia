using System;
using UnityEngine;

namespace _2___Scripts.Global.Events
{
    internal interface IOnPlayerEnteredEvent
    {
        public static event Action<IOnPlayerEnteredEvent> OnPlayerEnteredEvent;
        public EventType eventType { get; }
        public void Remove();
        public int numericData { get; }
        public string sceneName { get; }
        public string objectName { get; }

        public enum EventType
        {
            None,
            CoinCollected,
            DoubleJumpCollected,
            DashCollected,
            LongDashCollected,
            SpatialDashCollected,
            TimeGateCollected, //save anywhere
            WallJumpCollected,
            SwimUnderwaterCollected,
            SlowmotionFocusCollected, //debug only? Might not be fun mid actual game.
            MidAirFocusCollected, // might be deleted later or turned baseline ability.
            ChargeSpellCollected // prepared spells (not focus) can be charged to increase their damage
        }
    }
}
