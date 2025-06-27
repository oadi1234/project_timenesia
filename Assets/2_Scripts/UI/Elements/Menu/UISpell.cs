using System.Collections.Generic;
using _2_Scripts.Player;
using _2_Scripts.Spells;
using _2_Scripts.UI.Animation.Model;
using _2_Scripts.UI.Elements.HUD;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Elements.Menu
{
    public class UISpell : MonoBehaviour
    {
        private List<EffortType> effortCombination;
        private string spellName;
        private string description; //this should be displayed in the tooltip on hover/mouseover.
        private bool seen; //if spell has been added but cursor has not been moved over it will glow a bit.
        
        [SerializeField] private EffortBar effortBar;
        [SerializeField] private TMP_Text tmpSpellName;
        [SerializeField] private Button button;
        private SpellInventory spellInventory;
        private KnownSpellList knownSpellList;

        private void Awake()
        {
            effortBar.Initialize();
        }

        private void Initialize(List<EffortType> effortComb, string name, string description, bool seen = false)
        {
            this.effortCombination = effortComb;
            this.description = description;
            this.seen = seen;
            effortBar.SetMax(effortCombination.Count);
            effortBar.SetCurrent(effortCombination.Count);
            for (int i = 0; i < effortCombination.Count; i++)
            {
                effortBar.TrySetEffortType(effortComb[i], i);
            }

            spellName = name;
            tmpSpellName.text = name;
        }

        public void Initialize(List<EffortType> effortComb, BaseSpell spell)
        {
            Initialize(effortComb, spell.Name, spell.Name+BaseSpell.descriptionSuffix);
        }

        public int GetEffortCombinationLength()
        {
            return effortCombination.Count;
        }

        public List<EffortType> GetEffortCombination()
        {
            return new List<EffortType>(effortCombination);
        }

        public string GetName()
        {
            return name;
        }

        public bool Seen()
        {
            return seen;
        }

        public void SetSpellInventory(SpellInventory inventory)
        {
            spellInventory = inventory;
        }

        public void SetKnownSpellList(KnownSpellList knownSpellList)
        {
            this.knownSpellList = knownSpellList;
        }

        public void SetPreparedOnInitialize()
        {
            button.interactable = false;
        }

        public void TryAssignAsPreparedSpell()
        {
            int slot = spellInventory.AddPreparedSpell(effortCombination, spellName, description);
            if (slot == -1) return;
            knownSpellList.AddToCurrentPreparedSpells(this, slot);
            button.interactable = false;
        }

        public void ClearPreparedSpellAssignment()
        {
            button.interactable = true;
        }
    }
}