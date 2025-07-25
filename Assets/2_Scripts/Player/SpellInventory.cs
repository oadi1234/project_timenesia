using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Global.Events;
using _2_Scripts.Global.Events.Model;
using _2_Scripts.Spells;
using _2_Scripts.UI.Animation.Model;
using _2_Scripts.UI.Elements.Menu;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class SpellInventory : MonoBehaviour
    {
        [SerializeField] private KnownSpellList knownSpellList;
        [SerializeField] private PreparedSpellsPanel preparedSpellsPanel;
        
        private readonly Dictionary<int, List<EffortType>> preparedSpellsPerSlot = new()
        {
            {1, Spellbook.StringToEffortCombination("k") },
            {2, Spellbook.StringToEffortCombination("ke") },
            {3, Spellbook.StringToEffortCombination("a") }
        };

        private const int MaxPreparedSpells = 4;

        private readonly List<List<EffortType>> allKnownSpells = new()
        {
            Spellbook.StringToEffortCombination("k"),
            Spellbook.StringToEffortCombination("ke"),
            Spellbook.StringToEffortCombination("a")
        };

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            OnPlayerEnteredEvent.OnPlayerEntered += UnlockSpell;
            // TODO load from Game Data Manager.
            ClearKnownSpellsList();
            AssignCurrentKnownSpellsToUI();
            AssignCurrentPreparedSpellsToUI();
        }

        public bool HasSpellAtSlot(int slot)
        {
            return preparedSpellsPerSlot.ContainsKey(slot);
        }

        public List<EffortType> GetSpellAtSlot(int slot)
        {
            return preparedSpellsPerSlot.TryGetValue(slot, out var spell) ? spell : new List<EffortType>();
        }

        public int AddPreparedSpell(List<EffortType> effortCombination, string spellName, string description)
        {
            if (IsPreparedSpellsFull())
            {
                preparedSpellsPanel.NoSlotsWarning();
                return -1;
            }

            var slot = GetNextFreeSlot();
            preparedSpellsPanel.AssignPreparedSpellToSlot(effortCombination, spellName, description, slot);
            preparedSpellsPerSlot.Add(slot, effortCombination);
            return slot;
        }

        public BaseSpell TryGetSpellFromInventory(List<EffortType> effortCombination)
        {
            if (KnownSpellsContains(effortCombination))
            {
                return Spellbook.Instance.GetSpellCastData(effortCombination);
            }
            return Spellbook.Instance.GetWildMagic(effortCombination.Count);
        }

        private void AddKnownSpell(List<EffortType> effortCombination)
        {
            if (KnownSpellsContains(effortCombination)) return;
            allKnownSpells.Add(effortCombination);
            AddKnownSpellToUIList(effortCombination);
        }

        public void RemovePreparedSpell(int slot)
        {
            preparedSpellsPerSlot.Remove(slot);
            knownSpellList.ClearUISpellFromPrepared(slot);
        }

        public bool KnownSpellsContains(List<EffortType> effortCombination)
        {
            return allKnownSpells.Any(combination => combination.SequenceEqual(effortCombination));
        }

        private void ClearKnownSpellsList()
        {
            knownSpellList.RemoveChildren();
        }

        private void AddKnownSpellToUIList(List<EffortType> effortCombination)
        {
            knownSpellList.AddSpellToList(effortCombination);
        }

        private bool IsPreparedSpellsFull()
        {
            return preparedSpellsPerSlot.Count >= MaxPreparedSpells;
        }

        private void UnlockSpell(IOnPlayerEnteredEvent obj)
        {
            if (obj.collectedEventType == CollectedEventType.SpellCollected)
            {
                AddKnownSpell(Spellbook.IntToEffortCombination(obj.numericData));
            }
        }

        private void AssignCurrentPreparedSpellsToUI()
        {
            foreach (var kvp in preparedSpellsPerSlot)
            {
                var spellData = Spellbook.Instance.GetSpellDataForUI(kvp.Value);
                preparedSpellsPanel.AssignPreparedSpellToSlot(kvp.Value, spellData.Name, spellData.Name+BaseSpell.descriptionSuffix, kvp.Key);
                knownSpellList.PrepareSpellOnInitializationIfPresent(kvp.Value, kvp.Key);
            }
        }

        private void AssignCurrentKnownSpellsToUI()
        {
            foreach (var spell in allKnownSpells)
            {
                AddKnownSpellToUIList(spell);
            }
        }

        private int GetNextFreeSlot()
        {
            for (int i = 1; i <= MaxPreparedSpells; i++)
            {
                if (!preparedSpellsPerSlot.ContainsKey(i))
                {
                    return i;
                }
            }

            return 0;
        }
    }
}
