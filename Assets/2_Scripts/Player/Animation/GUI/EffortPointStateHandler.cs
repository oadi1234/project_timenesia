using System.Collections.Generic;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.model;
using UnityEngine;

namespace _2_Scripts.Player.Animation.GUI
{
    public class EffortPointStateHandler : MonoBehaviour, IStateHandler
    {
        public EffortType currentType; //tbh no clue if this is even necessary here, TODO delete if not needed

        private readonly Dictionary<EffortType, int> typeToIntDict = new()
        {
            { EffortType.Aether, AC.Aether },
            { EffortType.Empty, AC.Empty },
            { EffortType.Entropy, AC.Entropy },
            { EffortType.Raw, AC.Raw },
            { EffortType.Kinesis, AC.Kinesis },
            { EffortType.Mind, AC.Mind },
            { EffortType.Rune, AC.Rune },
            { EffortType.EndOfInput, AC.Empty },
            { EffortType.NoInput, AC.Empty }
        };

        public int currentState;

        public void SetCurrentState(EffortType effortType)
        {
            currentType = effortType; //as with line 10, todo delete if not needed
            currentState = typeToIntDict[effortType];
        }

        public int GetCurrentState()
        {
            return currentState;
        }

        public int GetCurrentHurtState()
        {
            return AC.None;
        }

        public bool LockXFlip()
        {
            return false;
        }

        public bool ShouldRestartAnim()
        {
            return false;
        }
    }
}