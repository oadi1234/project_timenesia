using System;
using System.Collections.Generic;
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
        [SerializeField] private List<EffortType> castCombination;
        [SerializeField] private PlayerInputManager inputManager;

        private int maxEffort; //temp starting values, make load from save later on.
        private int currentEffort;

        private int spellCapacity; //might get deleted.
        // private List<Spell> preparedSpells;
        // private List<BaseSpell> preparedSpells; //this feels like it should be somewhere else, not in effort but spellbook

        private int currentCastCombinationIndex;

        private float currentTime = 0f;
        private bool moved = false;

        private EffortType inputEffortType = EffortType.NoInput;

        public event Action<List<EffortType>> SpellCast;

        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            maxEffort = GameDataManager.Instance.currentGameData.MaxEffort;
            spellCapacity = GameDataManager.Instance.currentGameData.SpellCapacity;
            currentEffort = 0;
            effortBar.Initialize();
            effortBar.SetMax(maxEffort);
            effortBar.SetSpellCapacity(spellCapacity);
            castCombination = new List<EffortType>();
            inputManager.InputReceived += () => { moved = true; };

            currentCastCombinationIndex = 0;
        }

        private void FixedUpdate()
        {
            if (currentEffort >= maxEffort) return;
            currentTime += Time.fixedDeltaTime *
                           (inputManager.IsInConcentrationMode() ? concentrationEffortRegenMultiplier : 1f);
            if (!(currentTime > regenInterval)) return;
            currentTime = 0f;
            RecoverEffort(effortPerInterval);
        }

        private void Update()
        {
            if (moved)
            {
                moved = false;
                CleanData();
            }
            if (inputEffortType == EffortType.NoInput)
            {
                return;
            }

            if (currentCastCombinationIndex == 0 && inputEffortType == EffortType.EndOfInput)
            {
                inputManager.SetConcentration(false);
                return;
            }
            if (inputEffortType == EffortType.EndOfInput)
            {
                CastSpell();
                return;
            }

            if (currentCastCombinationIndex == spellCapacity-1)
            {
                currentCastCombinationIndex++;
                castCombination.Add(inputEffortType);
                CastSpell();
                //if last slot in cast sequence is already filled spell is cast automatically
                // this might actually not be a good idea and it would be better to cast it the moment we let go of concentration
                // but no clue how it would work with sustained spells. For sustained holding shift would be better.
                return;
            }

            if (effortBar.TrySetEffortType(inputEffortType, currentCastCombinationIndex))
            {
                castCombination.Add(inputEffortType);
                currentCastCombinationIndex++;
            }
        }

        public void SpellInput(EffortType concentrationInput)
        {
            inputEffortType = concentrationInput;
        }

        private void CastSpell()
        {
            // string debugString = "";
            // for (int i = 0; i < castCombination.Count; i++)
            // {
            //     debugString += castCombination[i] + " ";
            // }
            // Debug.Log("Player effort - cast combination: " + debugString);
            SpellCast?.Invoke(castCombination);
            CleanData();
        }

        private void CleanData()
        {
            effortBar.CleanManaSources();
            castCombination.Clear();
            currentCastCombinationIndex = 0;
            inputManager.SetConcentration(false);
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