using System;
using System.Collections.Generic;
using _2_Scripts.Global.Animation.Model;
using _2_Scripts.Player.Animation.model;
using UnityEngine;

namespace _2_Scripts.Global.Animation
{
    public class SingleAnimationStateHandler : MonoBehaviour, IStateHandler
    {
        [SerializeField] private AnimationName animationName;

        #region enum_to_animation_hash

        private readonly Dictionary<AnimationName, int> animNameToHash = new()
        {
            { AnimationName.SpellImpact, AC.SpellImpact },
            { AnimationName.SpellWandBlast, AC.SpellWandBlast },
            { AnimationName.ShieldParticle, AC.ShieldParticle},
            { AnimationName.ShieldStart, AC.ShieldStart},
            { AnimationName.ShieldEnd, AC.ShieldEnd}
        };

        #endregion

        private int currentAnimationHash;

        private void Awake()
        {
            currentAnimationHash = animNameToHash.GetValueOrDefault(animationName, AC.None);
        }

        public int GetCurrentState()
        {
            return currentAnimationHash;
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