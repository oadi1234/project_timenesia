using System.Collections.Generic;
using _2_Scripts.Player.Animation.model;
using UnityEngine;

namespace _2_Scripts.Player.Spell.weaponData
{
    public class StaffSpellOrigin : WeaponAnimationSpellOrigin
    {
        public StaffSpellOrigin()
        {
            AnimationSpellOriginPositions = new Dictionary<WeaponAnimationState, Vector3>()
            {
                { WeaponAnimationState.SpellBolt, new Vector3(-2.91f, 0.87f, 0)},
                { WeaponAnimationState.SpellBoltUp, new Vector3(-0.26f, 3.52f, 0)},
                { WeaponAnimationState.SpellBoltDown, new Vector3(0.25f, -2.08f, 0)}
            };
        }
    }
}