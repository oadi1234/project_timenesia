using System;
using System.Collections.Generic;
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
        public PlayerAbilityAndStats stats;
        public SceneDataHolder sceneDataHolder;
        public string LastSavePoint { get; private set; }

        public static GameDataManager Instance { get; private set; }
        public static event Action<IBaseEvent> OnCollected;
        /* objects to set inactive or change their state on scene load if they were destroyed/collected
         * set as 'key - value' in form of 'scene name - LoadObjectSpecification' */

        private TextMeshProUGUI CoinText;
        private string fileName = "save_00"; //this should be set after choosing from main menu.


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
            if (!sceneDataHolder)
                sceneDataHolder = ScriptableObject.CreateInstance<SceneDataHolder>();
            GameObject.Find("Player").GetComponent<PlayerMovementController>().SetVariablesOnLoad(ref stats);
            GameObject.Find("SceneLoader").GetComponent<SceneLoader>().SetSceneDataHolder(ref sceneDataHolder);
            CoinText = GameObject.Find("CoinCounter").GetComponent<TextMeshProUGUI>();
            DontDestroyOnLoad (gameObject); // this line might be deleted - game data manager is part of persistent scene now anyway.
            OnPlayerEnteredEvent.OnPlayerEntered += OnPlayer_Entered;
            Loadpoint.OnLoad += Load;
        }
    
        private void OnPlayer_Entered(IOnPlayerEnteredEvent obj)
        {
            switch (obj.eventType)
            {
                case IOnPlayerEnteredEvent.EventType.CoinCollected:
                    GainCoins(obj.numericData);
                    obj.Remove();
                    // Debug.Log("COINY: " + Coins);
                    //alterObjectLoadingDict.Add(obj.sceneName, new LoadObjectSpecification(obj.objectName, LoadObjectSpecification.OnSceneLoadBehaviour.INACTIVE));
                    sceneDataHolder.AddData(obj.sceneName, obj.objectName);
                    CoinText.text = stats.Coins.ToString();
                    break;
                case IOnPlayerEnteredEvent.EventType.DashCollected:
                    obj.Remove();
                    stats.UnlockAbility(AbilityName.Dash);
                    sceneDataHolder.AddData(obj.sceneName, obj.objectName, LoadObjectBehaviour.SAVE_PERSIST);
                    break;
                case IOnPlayerEnteredEvent.EventType.DoubleJumpCollected:
                    obj.Remove();
                    stats.UnlockAbility(AbilityName.DoubleJump);
                    sceneDataHolder.AddData(obj.sceneName, obj.objectName, LoadObjectBehaviour.SAVE_PERSIST);
                    break;
                case IOnPlayerEnteredEvent.EventType.WallJumpCollected:
                    obj.Remove();
                    stats.UnlockAbility(AbilityName.WallJump);
                    sceneDataHolder.AddData(obj.sceneName, obj.objectName, LoadObjectBehaviour.SAVE_PERSIST);
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
            AssignAlteredObjectDataToSceneData(saveData);

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

        private void AssignAlteredObjectDataToSceneData(SaveDataSchema saveData)
        {
            sceneDataHolder.setAlteredObjectsOnLoad(saveData.alteredObjects);
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
    }
}
