using _2_Scripts.Player;
using Pathfinding;

namespace _2_Scripts.Global
{
    public class AIDestinationSetterWithPlayerPosition : VersionedMonoBehaviour
    {
        public AIDestinationSetter AIDestinationSetter;

        void OnEnable()
        {
            AIDestinationSetter.target = PlayerPosition.GetPlayerTransform();
        }
    }
}