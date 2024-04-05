using System;
using System.Collections.Generic;
using System.Drawing;
using _2___Scripts.Global.Events;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace _2___Scripts.Global
{
    public class GameDataManager : MonoBehaviour
    {
        private GameDataManager()
        {
        }

        public SaveManager saveManager;
        public PlayerAbilityAndStats stats;
        public SceneLoader sceneLoader;

        public static GameDataManager Instance { get; private set; }
        public static event Action<IBaseEvent> OnCollected;

        private TextMeshProUGUI CoinText;
        public static event Action<AbilityName> OnAbilityLoad;

        public Vector2 LastSavePointPosition;


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
            //GameObject.Find("Player").GetComponent<PlayerMovementController>().SetVariablesOnLoad(ref stats); //move responsibility to load stats to player
            //CoinText = GameObject.Find("CoinCounter").GetComponent<TextMeshProUGUI>(); // see above
            stats = new PlayerAbilityAndStats();
            LastSavePointPosition = Vector2.zero;
            DontDestroyOnLoad (gameObject);
            OnPlayerEnteredEvent.OnPlayerEntered += OnPlayer_Entered;
            LoadButton.Load += Load;
            StartAction.NewGame += SetGameStateForNewGame;
        }

        private void SetGameStateForNewGame()
        {
            PurgeStats();
            SetStatsForNewGameStart();
            sceneLoader.InitialLoad("Scene0_0");
        }
    
        private void OnPlayer_Entered(IOnPlayerEnteredEvent obj)
        {
            switch (obj.eventType)
            {
                case IOnPlayerEnteredEvent.EventType.CoinCollected:
                    GainCoins(obj.numericData);
                    obj.Remove();
                    SceneDataHolder.instance.AddData(obj.sceneName, obj.objectName);
                    CoinText.text = stats.Coins.ToString();
                    break;
                case IOnPlayerEnteredEvent.EventType.DashCollected:
                    obj.Remove();
                    stats.UnlockAbility(AbilityName.Dash);
                    SceneDataHolder.instance.AddData(obj.sceneName, obj.objectName);
                    saveManager.PersistObjectLoadingStrategy(obj.sceneName, obj.objectName);
                    break;
                case IOnPlayerEnteredEvent.EventType.DoubleJumpCollected:
                    obj.Remove();
                    stats.UnlockAbility(AbilityName.DoubleJump);
                    SceneDataHolder.instance.AddData(obj.sceneName, obj.objectName);
                    saveManager.PersistObjectLoadingStrategy(obj.sceneName, obj.objectName);
                    break;
                case IOnPlayerEnteredEvent.EventType.WallJumpCollected:
                    obj.Remove();
                    stats.UnlockAbility(AbilityName.WallJump);
                    SceneDataHolder.instance.AddData(obj.sceneName, obj.objectName);
                    saveManager.PersistObjectLoadingStrategy(obj.sceneName, obj.objectName);
                    break;
            }
        }

        private void Load(string fileName)
        {
            // TODO Start loading animation here
            SaveDataSchema data = saveManager.Load(fileName);
            sceneLoader.InitialLoad(data.previewStatsDataSchema.sceneName);
            AssignDataFromSave(data);
            LastSavePointPosition = new Vector2(data.previewStatsDataSchema.savePointX, data.previewStatsDataSchema.savePointY);
            // TODO end loading animation here
            // TODO Start game entry animation, if one exists. Might be cool for simple and quick cinematics on game load.
            //GameObject.Find("Player").GetComponent<PlayerMovementController>().SetVariablesOnLoad(ref stats); //move responsibility for data retrieval to Player
            Console.WriteLine("loaded");
        }
    
        public void AssignDataFromSave(SaveDataSchema saveData)
        {
            AssignLoadDataToAbilities(saveData);
            AssignLoadDataToCurrencies(saveData);
            AssignLoadDataToStats(saveData);
            AssignLoadDataToObjectLoadingStrategy(saveData);

            //LastSavePoint = saveData.SavePoint;
        }

        #region SAVE_DATA_ASSIGNMENT
        private void AssignLoadDataToStats(SaveDataSchema saveData)
        {
            stats.MaxHealth = stats.CurrentHealth = saveData.previewStatsDataSchema.MaxHealth;
            stats.MaxEffort = saveData.previewStatsDataSchema.MaxEffort;
            // TODO reload UI elements here
        }

        private void AssignLoadDataToCurrencies(SaveDataSchema saveData)
        {
            stats.Coins = saveData.gameStateSaveDataSchema.Coins;
            CoinText.text = stats.Coins.ToString();
        }

        private void AssignLoadDataToAbilities(SaveDataSchema saveData)
        {
            stats.abilities = saveData.gameStateSaveDataSchema.abilities;
            foreach (var ability in stats.abilities)
            {
                if(ability.Value)
                {
                    OnAbilityLoad(ability.Key); // TODO in the future all loading and saving should be moved to coroutines to avoid stuttering on both. It will also make it easier to control if we loaded everything.
                }
            }
        }

        private void AssignLoadDataToObjectLoadingStrategy(SaveDataSchema saveData)
        {
            SceneDataHolder.instance.SetLoadStrategyOnGameLoad(saveData.gameStateSaveDataSchema.alteredObjects);
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

        public void PurgeStats()
        {
            //Used mostly to clear save data when going to menu.
            stats = new PlayerAbilityAndStats();
        }

        public void SetStatsForNewGameStart()
        {
            stats.MaxHealth = 3;
            stats.MaxEffort = 2;
            stats.SpellCapacity = 2;
            LastSavePointPosition = new Vector2(0f, 0f); // this might need changing later. Still, (0, 0) is a good starting point I think.
        }
    }
}
