using System.Collections.Generic;
using _2_Scripts.Player;
using _2_Scripts.UI.Animation.Model;
using _2_Scripts.UI.Elements.HUD;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Elements.Menu
{
    public class UIPreparedSpell : MonoBehaviour
    {
        [SerializeField] private TMP_Text textObject;
        [SerializeField] private EffortBar effortBar;
        [SerializeField] private SpellInventory spellInventory;
        [SerializeField] private int slot;
        [SerializeField] private Button button;
        private bool filled = false;
        private string tooltipDescription;
        
        private void Awake()
        {
            effortBar.Initialize();
            
        }
        
        public void AddSpell(List<EffortType> effortCombination, string spellName, string description)
        {
            if (filled) return;
            button.interactable = true;
            effortBar.SetMax(effortCombination.Count);
            effortBar.SetCurrent(effortCombination.Count);
            for (int i = 0; i < effortCombination.Count; i++)
            {
                effortBar.TrySetEffortType(effortCombination[i], i);
            }
            textObject.text = spellName;
            tooltipDescription = description;
            filled = true;
        }

        public void RemoveSpell()
        {
            spellInventory.RemovePreparedSpell(slot);
            button.interactable = false;
            ClearSpellData();
        }
        
        private void ClearSpellData()
        {
            textObject.text = "";
            tooltipDescription = "";
            effortBar.SetMax(0);
            filled = false;
        }
        
    }
}
