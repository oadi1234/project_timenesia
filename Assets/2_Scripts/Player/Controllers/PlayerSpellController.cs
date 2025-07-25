using System;
using System.Collections.Generic;
using _2_Scripts.Player.Statistics;
using _2_Scripts.Spells;
using _2_Scripts.UI.Animation.Model;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class PlayerSpellController : MonoBehaviour
    {
        [SerializeField] private PlayerEffort playerEffort;
        [SerializeField] private SpellInventory spellInventory;
        private SpellType spellType;
        private BaseSpell currentSpellcast;
        private int spellCost;
        
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
            spellCost = effortCombination.Count;
            if (playerEffort.CanUseEffort(spellCost))
            {
                currentSpellcast = spellInventory.TryGetSpellFromInventory(effortCombination);
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

        public void UseSpellCost()
        {
            playerEffort.UseEffort(spellCost);
        }
    }
}
