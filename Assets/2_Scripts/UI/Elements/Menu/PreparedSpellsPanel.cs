using System.Collections.Generic;
using _2_Scripts.Player;
using _2_Scripts.UI.Animation.Model;
using UnityEngine;

namespace _2_Scripts.UI.Elements.Menu
{
    // TODO: what this does:
    //  players will be able to assign spells here. It should essentially cover the following functions:
    //  1. display empty panel with just the keyboard shortcut when nothing is assigned here.
    //  2. pressing a spell on spell list should assign it to the next empty panel. It should display relevant information
    //  Ad.2. I think it would be better for the user to manually assign a spell to a given panel, or give a possibility
    //        to change their arrangement afterwards.
    //  3. Pressing one of prepared spells should free its slot.
    //  4. pressing a spell in list with all slots filled should provide visual feedback that it cant be done - both
    //     the spell in list and prepared spells panel should flash red and tremble a bit.
    //  this class handles assigning spells to Prepared spell classes, which then update individual slots.
    //  it only does the visual part, the actual logic should be in SpellInventory.cs.
    
    public class PreparedSpellsPanel : MonoBehaviour
    {
        [SerializeField] private SpellInventory spellInventory;
        [SerializeField] private List<UIPreparedSpell> preparedSpellsSlots;

        private void Awake()
        {
            if (preparedSpellsSlots.Count != 4) Debug.LogError("Invalid number of slots. Received: " + preparedSpellsSlots.Count + ", expected 4");
        }
        
        public void AssignPreparedSpellToSlot(List<EffortType> effortCombination, string spellName, string description, int slot)
        {
            preparedSpellsSlots[slot-1].AddSpell(effortCombination, spellName, description);
        }

        public void NoSlotsWarning()
        {
            // TODO flash red and shake the panel a bit to show that no slots are available.
        }
        
    }
}
