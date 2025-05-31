using System.Collections.Generic;
using _2_Scripts.Global.Animation.Model;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.UI.Animation.Model;
using UnityEngine;

namespace _2_Scripts.UI.Animation
{
    public class EffortPointStateHandler : MonoBehaviour, IGUIPointStateHandler<EffortType>
    {
        private readonly Dictionary<EffortType, int> typeToIntDict = new()
        {
            { EffortType.Aether, AC.EffortAether },
            { EffortType.Empty, AC.EffortEmpty },
            { EffortType.Entropy, AC.EffortEntropy },
            { EffortType.Raw, AC.EffortRaw },
            { EffortType.Kinesis, AC.EffortKinesis },
            { EffortType.Mind, AC.EffortMind },
            { EffortType.Rune, AC.EffortRune },
            { EffortType.EndOfInput, AC.EffortEmpty },
            { EffortType.NoInput, AC.EffortEmpty }
        };

        public int currentState;

        public void SetCurrentState(EffortType effortType)
        {
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