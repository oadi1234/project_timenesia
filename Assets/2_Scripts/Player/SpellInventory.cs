using System.Collections.Generic;
using _2_Scripts.Player.model;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class SpellInventory : MonoBehaviour
    {
        // TODO make actual inventory functions.
        // player should both only know certain spells (so no full spellbook access) and have prepared spells assignable.

        private Dictionary<int, List<EffortType>> preparedSpells = new ()
        {
            { 1, Spellbook.StringToEfType("k")},
            { 2, Spellbook.StringToEfType("ke")},
            { 3, Spellbook.StringToEfType("a")},
        };

        private Dictionary<int, List<EffortType>> knownSpells = new(); //all known spells. TODO give functionality.

        public List<EffortType> GetSpellAtSlot(int slot)
        {
            return preparedSpells.TryGetValue(slot, out var spell) ? spell : new List<EffortType>();
        }
    }
}
