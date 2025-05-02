using System;
using System.Collections.Generic;
using _2_Scripts.Player.model;
using _2_Scripts.Player.Statistics;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class PlayerSpellController : MonoBehaviour
    {
        [SerializeField] private PlayerEffort playerEffort;
        private SpellType spellType;
        
        public event Action Spellcasted;
        
        private void Awake()
        {
            playerEffort.SpellCast += CastSpell;
        }

        private void CastSpell(List<EffortType> effortCombination)
        {
            if (playerEffort.UseEffort(effortCombination.Count))
            {
                spellType = Spellbook.Instance.GetSpellData(effortCombination).SpellType;
                Spellcasted?.Invoke();
            }
        }

        public void InputCastSpell(List<EffortType> effortCombination, bool isCasting)
        {
            if (isCasting)
            {
                CastSpell(effortCombination);
            }
        }
        
        public SpellType GetSpellType()
        {
            return spellType;
        }

        public void ClearSpellType()
        {
            spellType = SpellType.None;
        }
    }
}
