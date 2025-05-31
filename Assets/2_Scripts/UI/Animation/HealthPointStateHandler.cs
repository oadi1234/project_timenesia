using System.Collections.Generic;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.UI.Animation.Model;
using UnityEngine;

namespace _2_Scripts.UI.Animation
{
    public class HealthPointStateHandler : MonoBehaviour, IGUIPointStateHandler<HealthType>
    {
        private Dictionary<HealthType, int> healthToIntDict = new()
        {
            { HealthType.Empty, AC.HealthEmpty },
            { HealthType.Health, AC.HealthFull },
            { HealthType.Shield, AC.HealthShield }
        };

        private int currentState = 0;

        public void SetCurrentState(HealthType healthType)
        {
            currentState = healthToIntDict[healthType];
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