using _2_Scripts.Global;
using _2_Scripts.Player.model;
using _2_Scripts.Spells;
using _2_Scripts.UI.Elements.HUD;
using Spells;
using UnityEngine;

namespace _2_Scripts.Player.Statistics
{
    public class PlayerEffort : MonoBehaviour
    {
        [SerializeField]
        private float
            regenInterval =
                1f; // TODO set this depending on the chosen difficulty, or allow concentrations to change it. Might get moved to PlayerAbilityAndStats or something like it.

        [SerializeField] private int effortPerInterval = 1;
        [SerializeField] private float concentrationEffortRegenMultiplier = 2f;
        [SerializeField] private EffortBar effortBar;
        [SerializeField] private EffortType[] castCombination;
        [SerializeField] private PlayerInputManager inputManager;

        private int maxEffort; //temp starting values, make load from save later on.
        private int currentEffort;

        private int spellCapacity; //might get deleted.
        // private List<Spell> preparedSpells;
        // private List<BaseSpell> preparedSpells; //this feels like it should be somewhere else, not in effort but spellbook

        private int currentCastCombinationIndex;

        private float currentTime = 0f;

        void Start()
        {
            Initialize();
            // preparedSpells = new List<Spell> { new Spell("Fireball", new List<EffortElement> { EffortElement.Fire, EffortElement.Fire }) };
            // preparedSpells = new List<BaseSpell>  //this feels like it should be somewhere else, not in effort but spellbook
            // {
            // new BaseSpell("Fireball", 1, 1, 1, "", SpellCastEffects.Fireball),
            // new BaseSpell("Fireball", 1, 1, 1, "", SpellCastEffects.ShockWave),
            // }; //new List<EffortElement> { EffortElement.Fire, EffortElement.Fire }) };
        }

        // void Update()
        // {
        //     bool isFocusing = CheckFocusBegin();
        //     if (isFocusing)
        //     {
        //         FocusCasting();
        //     }
        //     if (Input.GetKeyUp(KeyCode.LeftShift))
        //     {
        //         CastSpell();
        //     }
        // }

        private void Initialize()
        {
            maxEffort = GameDataManager.Instance.currentGameData.MaxEffort;
            spellCapacity = GameDataManager.Instance.currentGameData.SpellCapacity;
            currentEffort = 0;
            effortBar.Initialize();
            effortBar.SetMax(maxEffort);
            effortBar.SetSpellCapacity(spellCapacity);
            castCombination = new EffortType[spellCapacity]; //TODO on spell capacity increase make new array
            ClearCastCombination();

            currentCastCombinationIndex = 0;
        }

        // private void CastSpell(int indexOfSpell) // TODO Move to a different class - player effort should only manage effort levels. Spellbook class might be better for casting spells.
        // {
        //     // var spell = preparedSpells[indexOfSpell];
        //     spell.CastHandler();
        //     // for (int i = 0; i < spell.EffortElements.Length; i++)
        //     // {
        //     //     effortBar.SetElement(spell.EffortElements[i], i);
        //     // }
        //     //TODO - there will be animation cast time, so this should go somewhere here, as it would let us see effort used
        //     //lastPreparedSpellManaCost = spell;
        // }

        // private void CastSpell()
        // {
        //     UseEffort(currentCastCombinationIndex);
        //     CleanSourceCombinations();
        //     //TODO: clearColors
        // }

        // private void CleanSourceCombinations()
        // {
        //     for(int i = 0; i < currentCastCombinationIndex; i++)
        //     {
        //         castCombination[i] = EffortType.Empty;
        //     }
        //     currentCastCombinationIndex = 0;
        // }

        // private void FocusCasting()
        // {
        //     if (currentCastCombinationIndex < spellCapacity && effortBar.GetEffortElementAt(currentCastCombinationIndex) == EffortType.Raw)
        //     {
        //         
        //     }
        // }

        private void AddNewSourceWhileFocusing(EffortType source)
        {
            if (currentCastCombinationIndex < castCombination.Length)
            {
                castCombination[currentCastCombinationIndex] = source;
                effortBar.SetEffortType(source, currentCastCombinationIndex);
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

        public void SpellInput(EffortType inputEffortType)
        {
            if (inputEffortType == EffortType.NoInput)
            {
                return;
            }
            if (currentCastCombinationIndex == 0 && inputEffortType == EffortType.EndOfInput)
                return;
            if (inputEffortType == EffortType.EndOfInput)
            {
                CastSpell();
                return;
            }

            if (currentCastCombinationIndex == spellCapacity-1)
            {
                currentCastCombinationIndex++;
                CastSpell(); //if last slot in cast sequence is already filled spell is cast automatically
                // this might actually not be a good idea and it would be better to cast it the moment we let go of concentration
                // but no clue how it would work with sustained spells. For sustained holding shift would be better.
                inputManager.SetConcentration(false);
                return;
            }

            if (effortBar.TrySetEffortType(inputEffortType, currentCastCombinationIndex))
            {
                castCombination[currentCastCombinationIndex] = inputEffortType;
                currentCastCombinationIndex++;
            }
        }

        private void ClearCastCombination()
        {
            for (int i = 0; i < castCombination.Length; i++)
            {
                castCombination[i] = EffortType.NoInput;
            }
        }

        private void CastSpell()
        {
            UseEffort(currentCastCombinationIndex);
            Debug.Log("Spell was cast.");
            effortBar.CleanManaSources();
            ClearCastCombination();
            currentCastCombinationIndex = 0;
        }

        void FixedUpdate()
        {
            if (currentEffort < maxEffort)
            {
                currentTime += Time.fixedDeltaTime *
                               (inputManager.IsConcentrating() ? concentrationEffortRegenMultiplier : 1f);
                if (currentTime > regenInterval)
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

            currentEffort -= cost;
            effortBar.SetCurrent(currentEffort);
            effortBar.CleanManaSources();
            return true;
        }

        public void RecoverEffort(int amount)
        {
            currentEffort = Mathf.Min(currentEffort + amount, maxEffort);
            effortBar.SetCurrent(currentEffort);
        }
    }
}