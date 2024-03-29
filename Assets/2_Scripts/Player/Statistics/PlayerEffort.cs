using System.Collections.Generic;
using _2___Scripts.UI;
using Spells;
using UnityEngine;

namespace _2___Scripts.Player
{
    public class PlayerEffort : MonoBehaviour
    {
        public int maxEffort = 4; //temp starting values, make load from save later on.
        public int currentEffort = 0;
        public int spellCapacity = 2; //might get deleted.

        public float regenInterval = 0.8f; // TODO set this depending on the chosen difficulty, or allow concentrations to change it. Might get moved to PlayerAbilityAndStats or something like it.
        public int effortPerInterval = 1;
        public EffortBar effortBar;
        public EffortType[] castCombination;
        // private List<Spell> preparedSpells;
        private List<BaseSpell> preparedSpells;
        private int lastPreparedSpellManaCost = -1;

        private int currentCastCombinationIndex;

        private float currentTime = 0f;

        void Start()
        {
            effortBar.Initialize();
            effortBar.SetMaxEffort(maxEffort, false);
            effortBar.SetSpellCapacity(spellCapacity);
            castCombination = new EffortType[maxEffort];
            for (int i = 0; i < castCombination.Length; i++)
            {
                castCombination[i] = EffortType.Empty;
            }
            currentCastCombinationIndex = 0;
            // preparedSpells = new List<Spell> { new Spell("Fireball", new List<EffortElement> { EffortElement.Fire, EffortElement.Fire }) };
            preparedSpells = new List<BaseSpell>
            {
                new BaseSpell("Fireball", 1, 1, 1, "", SpellCastEffects.Fireball),
                new BaseSpell("Fireball", 1, 1, 1, "", SpellCastEffects.ShockWave),
            }; //new List<EffortElement> { EffortElement.Fire, EffortElement.Fire }) };

        }

        void Update()
        {
            bool isFocusing = CheckFocusBegin();
            if (isFocusing)
            {
                FocusCasting();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                CastSpell();
            }

            //else
            //{
            //    if (Input.GetKeyDown(KeyCode.H))
            //    {
            //        CastSpell(0);
            //    }
            //    if (Input.GetKeyDown(KeyCode.J))
            //    {
            //        CastSpell(1);
            //    }
            //    if (Input.GetKeyDown(KeyCode.K))
            //    {
            //        UseEffort(3);
            //    }

            //    if (Input.GetKeyUp(KeyCode.H))
            //    {
            //        UseEffort(lastPreparedSpellManaCost);
            //    }
            //}
        }

        private void CastSpell(int indexOfSpell)
        {
            var spell = preparedSpells[indexOfSpell];
            spell.CastHandler();
            // for (int i = 0; i < spell.EffortElements.Length; i++)
            // {
            //     effortBar.SetElement(spell.EffortElements[i], i);
            // }
            //TODO - there will be animation cast time, so this should go somewhere here, as it would let us see effort used
            //lastPreparedSpellManaCost = spell;
        }

        private void CastSpell()
        {
            int manaCost = CalculateManaCost();
            UseEffort(manaCost);
            CleanSourceCombinations();
            //TODO: clearColors
        }

        private void CleanSourceCombinations()
        {
            for(int i = 0; i < currentCastCombinationIndex; i++)
            {
                castCombination[i] = EffortType.Empty;
            }
            currentCastCombinationIndex = 0;
        }

        private int CalculateManaCost()
        {
            //int cost = 0;
            //foreach(var source in castCombination)
            //{
            //    if (source != EffortElement.Empty)
            //        cost++;
            //    else
            //        break;
            //}
            //return cost;
            return currentCastCombinationIndex;
        }

        private void FocusCasting()
        {
            if (currentCastCombinationIndex < maxEffort && effortBar.GetEffortElementAt(currentCastCombinationIndex) == EffortType.Raw)
            {
                if (Input.GetKeyDown(KeyCode.O))
                {
                    AddNewSourceWhileFocusing(EffortType.Cold);
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
                    AddNewSourceWhileFocusing(EffortType.Fire);
                }
            }
        }

        private void AddNewSourceWhileFocusing(EffortType source)
        {
            if (currentCastCombinationIndex < castCombination.Length)
            {
                castCombination[currentCastCombinationIndex] = source;
                effortBar.SetEffortAnimation(source, currentCastCombinationIndex);
                currentCastCombinationIndex++;
            }
            //    int index = 0;
            //    EffortElement currentSource = castCombination[index];
            //    while(currentSource != EffortElement.Empty)
            //    {
            //        index++;
            //        currentSource = castCombination[index];
            //    }
        }

        private bool CheckFocusBegin()
        {
            return Input.GetKey(KeyCode.LeftShift);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(currentEffort < maxEffort)
            {
                currentTime += Time.fixedDeltaTime;
                if(currentTime > regenInterval)
                {
                    currentTime = 0f;
                    RecoverEffort(effortPerInterval);
                }
            }
        }

    
        public bool UseEffort(int cost)
        {
            if (cost > currentEffort)
                return false;
            else
                currentEffort -= cost;
            effortBar.SetCurrentMana(currentEffort);
            effortBar.CleanManaSources();
            return true;
        }

        //Freeze mana will sometimes keep rightmost mana points coloured - they cannot be used and
        //are not recovering. Possibly used when a persistent effect is present (i.e. shield from force element).
        public void FreezeMana(int cost)
        {

        }

        public void RecoverEffort(int amount)
        {
            currentEffort += amount;
            if (currentEffort > maxEffort)
                currentEffort = maxEffort;
            effortBar.SetCurrentMana(currentEffort);
        }
    }
}
