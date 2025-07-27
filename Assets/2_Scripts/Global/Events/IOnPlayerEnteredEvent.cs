using _2_Scripts.Global.Events.Model;

namespace _2_Scripts.Global.Events
{
    public interface IOnPlayerEnteredEvent
    {
        public CollectedEventType collectedEventType { get; }
        public void Remove();
        public int numericData { get; }
        public string sceneName { get; }
        public string objectName { get; }
    }
}
