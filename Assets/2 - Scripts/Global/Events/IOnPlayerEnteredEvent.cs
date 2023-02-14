using System;
using Assets.Scripts.Enemies.Attacks;

namespace _2___Scripts.Global.Events
{
    internal interface IOnPlayerEnteredEvent
    {
        public static event Action<IOnPlayerEnteredEvent> OnPlayerEnteredEvent;
        public string EventName { get; }
        public void Remove();
    }
}
