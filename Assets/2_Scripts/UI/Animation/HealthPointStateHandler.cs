using System.Collections.Generic;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.UI.Animation.Model;
using UnityEngine;

namespace _2_Scripts.UI.Animation
{
    public class HealthPointStateHandler : AbstractGUIPointStateHandler<HealthType>
    {
        private Dictionary<HealthType, int> healthToIntDict = new()
        {
            { HealthType.Empty, AC.HealthEmpty },
            { HealthType.Health, AC.HealthFull },
            { HealthType.Shield, AC.HealthShield }
        };
        
        public override void SetCurrentState(HealthType healthType)
        {
            CurrentState = healthToIntDict[healthType];
        }
    }
}