using System.Collections.Generic;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.UI.Animation.Model;

namespace _2_Scripts.UI.Animation
{
    public class EffortPointStateHandler : AbstractGUIPointStateHandler<EffortType>
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

        public override void SetCurrentState(EffortType effortType)
        {
            CurrentState = typeToIntDict[effortType];
        }
    }
}