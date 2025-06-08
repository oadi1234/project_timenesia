using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Global.Utility;
using _2_Scripts.Player;
using _2_Scripts.UI.Animation.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2_Scripts.UI.Elements.Menu
{
    public class KnownSpellList : MonoBehaviour
    {
        [SerializeField] private GameObject knownSpellListElement;
        [SerializeField] private SpellInventory spellInventory;
        [SerializeField] private RectTransform list;
        [FormerlySerializedAs("preparedSpellPanel")] [SerializeField] private PreparedSpellsPanel preparedSpellsPanel;
        
        private SortedBy sortedBy = SortedBy.Name;
        
        private readonly List<UISpell> spellUIData = new();
        private readonly Dictionary<int, UISpell> preparedSpells = new();
        
        public void SortUIDataByEffortCombinationLength()
        {
            spellUIData.Sort(new SpellUIEffortLengthComparer());
            sortedBy = SortedBy.EffortCombination;
            AssignCorrectIndexToListElements();
        }

        public void SortUIDataByName()
        {
            spellUIData.Sort(new SpellUINameComparer());
            sortedBy = SortedBy.Name;
            AssignCorrectIndexToListElements();
        }

        public void SortUIDataBySeen()
        {
            spellUIData.Sort(new SpellUISeenComparer());
            sortedBy = SortedBy.Seen;
            AssignCorrectIndexToListElements();
        }

        public void AddSpellToList(List<EffortType> effortCombination)
        {
            var spellData = Spellbook.Instance.GetSpellDataForUI(effortCombination);
            if (spellData.Name.Equals("invalid"))
            {
                Debug.Log("Invalid spell. Received effort: " + effortCombination.Aggregate("", (current, effort) => current + (effort + " "))); //TODO left for testing, remove.
                return;
            }
            
            var tempGameObject = Instantiate(knownSpellListElement, list, false);
            var uiSpell = tempGameObject.GetComponent<UISpell>();
            uiSpell.Initialize(effortCombination, spellData);
            uiSpell.SetSpellInventory(spellInventory);
            uiSpell.SetKnownSpellList(this);
            spellUIData.Add(uiSpell);
            
            SortSpellUIData();
        }

        public void ClearUISpellFromPrepared(int slot)
        {
            if (preparedSpells.TryGetValue(slot, out var spell))
                spell.ClearPreparedSpellAssignment();
            preparedSpells.Remove(slot);
        }

        public void AddToCurrentPreparedSpells(UISpell spell, int slot)
        {
            preparedSpells.Add(slot, spell);
        }

        public void RemoveChildren()
        {
            foreach (Transform child in list)
            {
                Destroy(child.gameObject);
            }
        }

        private void AssignCorrectIndexToListElements()
        {
            for (int i = 0; i < spellUIData.Count; i++)
            {
                spellUIData[i].transform.SetSiblingIndex(i);
            }
        }

        private void SortSpellUIData()
        {
            switch (sortedBy)
            {
                case SortedBy.EffortCombination:
                    SortUIDataByEffortCombinationLength();
                    break;
                case SortedBy.Seen:
                    SortUIDataBySeen();
                    break;
                case SortedBy.Name:
                default:
                    SortUIDataByName();
                    break;
            }
        }

        private enum SortedBy
        {
            Name,
            EffortCombination,
            Seen,
        }
    }
}
