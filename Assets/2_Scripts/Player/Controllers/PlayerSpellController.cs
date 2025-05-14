using System;
using System.Collections.Generic;
using _2_Scripts.Player.model;
using _2_Scripts.Player.Statistics;
using _2_Scripts.Spells;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class PlayerSpellController : MonoBehaviour
    {
        [SerializeField] private PlayerEffort playerEffort;
        private SpellType spellType;
        private BaseSpell currentSpellcast;
        
        public event Action<SpellType> Spellcasted;
        
        private void Awake()
        {
            playerEffort.SpellCast += CastSpell;
        }

        public void InvokeSpell()
        {
            currentSpellcast.CastHandler();
        }

        private void CastSpell(List<EffortType> effortCombination)
        {
            if (playerEffort.UseEffort(effortCombination.Count))
            {
                // Debug.Break();
                currentSpellcast = Spellbook.Instance.GetSpellData(effortCombination);
                spellType = currentSpellcast.SpellType;
                Spellcasted?.Invoke(spellType);
            }
        }

        public void CastHotkeySpell(List<EffortType> effortCombination, bool isCasting)
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
