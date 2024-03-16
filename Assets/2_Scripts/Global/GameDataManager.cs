using System;
using System.Drawing;
using _2___Scripts.Global.Events;
using TMPro;
using UnityEngine;

namespace _2___Scripts.Global
{
    public class GameDataManager : MonoBehaviour
    {
        private GameDataManager()
        {
        }

        public SaveManager saveManager;

        public static GameDataManager Instance { get; private set; }
        public static event Action<IBaseEvent> OnCollected;
    
        private TextMeshProUGUI CoinText;
        private string fileName = "save_00"; //this should be set after choosing from main menu.

        public PlayerAbilityAndStats stats;
        public string LastSavePoint  { get; private set; }
	
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            if(!stats)
                stats = ScriptableObject.CreateInstance<PlayerAbilityAndStats>();
            GameObject.Find("Player").GetComponent<PlayerMovementController>().SetVariablesOnLoad(ref stats);
            CoinText = GameObject.Find("CoinCounter").GetComponent<TextMeshProUGUI>();
            DontDestroyOnLoad (gameObject);
            OnPlayerEnteredEvent.OnPlayerEntered += OnPlayer_Entered;
            Loadpoint.OnLoad += Load;
            AbilityUnlocker.OnAbilityUnlocked += UnlockAbility;
        }
    
        private void OnPlayer_Entered(IOnPlayerEnteredEvent obj)
        {
            switch (obj.EventName)
            {
                case "CoinCollected":
                    GainCoins(obj.NumericData);
                    obj.Remove();
                    // Debug.Log("COINY: " + Coins);
                    CoinText.text = stats.Coins.ToString();
                    break;
            }
        }

        private void Load()
        {
            var data = saveManager.Load(fileName);
            LoadFromSave(data);
            GameObject.Find("Player").GetComponent<PlayerMovementController>().SetVariablesOnLoad(ref stats);
            Console.WriteLine("loaded");
        }
    
        public void LoadFromSave(SaveDataSchema saveData)
        {
            AssignLoadDataToAbilities(saveData);
            AssignLoadDataToCurrencies(saveData);
            AssignLoadDataToStats(saveData);

            LastSavePoint = saveData.SavePoint;
        }

        #region SAVE_DATA_ASSIGNMENT
        private void AssignLoadDataToStats(SaveDataSchema saveData)
        {
            stats.MaxHealth = stats.CurrentHealth = saveData.MaxHealth;
            stats.CurrentEffort = stats.MaxEffort = saveData.MaxEffort;
            // TODO reload UI elements here
        }

        private void AssignLoadDataToCurrencies(SaveDataSchema saveData)
        {
            stats.Coins = saveData.Coins;
            CoinText.text = stats.Coins.ToString();
        }

        private void AssignLoadDataToAbilities(SaveDataSchema saveData)
        {
            stats.abilities = saveData.abilities;
        }
        #endregion

        public bool TakeDamage(int amount)
        {
            if (amount >= stats.CurrentHealth)
            {
                stats.CurrentHealth = 0;
                return false;
            }
            //stats.CurrentHealth = Math.Max(0, stats.CurrentHealth - amount);
            //return stats.CurrentHealth > 0;
            stats.CurrentHealth -= amount;
            return true;
        }

        public void GainCoins(int coins = 1) => stats.Coins += coins;

        public void UnlockAbility(AbilityName abilityName)
        {
            stats.UnlockAbility(abilityName);
        }
    }
}
