using System.Collections.Generic;
using _2_Scripts.Global.Animation.Model;
using _2_Scripts.Player.Animation.model;
using UnityEngine;

namespace _2_Scripts.Global.Animation
{
    public class SingleAnimationStateHandler : AbstractStateHandler
    {
        [SerializeField] private AnimationName animationName;

        #region enum_to_animation_hash

        private readonly Dictionary<AnimationName, int> animNameToHash = new()
        {
            { AnimationName.SpellImpact, AC.SpellImpact },
            { AnimationName.SpellWandBlast, AC.SpellWandBlast },
            { AnimationName.ShieldParticle, AC.ShieldParticle},
            { AnimationName.ShieldStart, AC.ShieldStart},
            { AnimationName.ShieldEnd, AC.ShieldEnd},
            { AnimationName.WeaponWideSwoosh, AC.StaffBasicSwoosh}
        };

        #endregion

        private int currentAnimationHash;

        private void Awake()
        {
            currentAnimationHash = animNameToHash.GetValueOrDefault(animationName, AC.None);
        }

        public override int GetCurrentState()
        {
            return currentAnimationHash;
        }
    }
}