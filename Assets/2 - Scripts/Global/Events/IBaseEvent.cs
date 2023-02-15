using System;

namespace _2___Scripts.Global.Events
{
    public interface IBaseEvent
    {
        public static event Action<IBaseEvent> OnBaseEvent;
        public string EventName { get; }
        
    }
}