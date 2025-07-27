using System.Collections.Generic;
using _2_Scripts.Player.Animation.model;
using UnityEngine;

namespace _2_Scripts.Player.Spell
{
    public class WeaponAnimationSpellOrigin
    {
        protected Dictionary<WeaponAnimationState, Vector3> AnimationSpellOriginPositions;
        
        private static readonly Vector3 noOrigin = Vector3.zero;

        protected WeaponAnimationSpellOrigin()
        {
            
        }

        public Vector3 GetForState(WeaponAnimationState state)
        {
            return AnimationSpellOriginPositions.GetValueOrDefault(state, noOrigin);
        }
    }
}