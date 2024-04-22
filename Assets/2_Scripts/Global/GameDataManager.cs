using System;
using System.Collections.Generic;
using _2___Scripts.Global.Events;
using _2_Scripts.ExtensionMethods;
using _2_Scripts.Global.Events;
using _2_Scripts.Global.SaveSystem;
using _2_Scripts.Global.SaveSystem.SaveDataSchemas;
using _2_Scripts.Scenes;
using _2_Scripts.UI.Elements.HUD;
using _2_Scripts.UI.Elements.MainMenu;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2_Scripts.Global
{
    public class GameDataManager : MonoBehaviour
    {
        // public PlayerAbilitiesAndGameData GameData;
        public GameData currentGameData;

        public static GameDataManager Instance { get; private set; }
        public static event Action<IBaseEvent> OnCollected;
        
        public Vector2 lastSavePointPosition;
        public Savepoint lastSavepoint;
        public string directoryName;
        
        private GameDataManager()
        {
        }

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;

                lastSavePointPosition = Vector2.zero;
                DontDestroyOnLoad(gameObject);
                OnPlayerEnteredEvent.OnPlayerEntered += OnPlayer_Entered;
                LoadButton.LoadAction += Load;
                StartButton.NewGame += SetGameStateForNewGame;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            
            //GameObject.Find("Player").GetComponent<PlayerMovementController>().SetVariablesOnLoad(ref GameData); //move responsibility to load GameData to player
            //CoinText = GameObject.Find("CoinCounter").GetComponent<TextMeshProUGUI>(); // see above
        }
        
        public void PurgeGameData()
        {
            //Used mostly to clear save data when going to menu.
            currentGameData = new GameData();
        }

        public void SetGameDataForNewGameStart()
        {
            currentGameData.MaxHealth = 3;
            currentGameData.MaxEffort = 2;
            currentGameData.SpellCapacity = 2;
            lastSavePointPosition = new Vector2(0f, 0f); // this might need changing later. Still, (0, 0) is a good starting point I think.
        }
        
        private void SetGameStateForNewGame(string directoryName)
        {
            PurgeGameData();
            SetGameDataForNewGameStart();
            this.directoryName = directoryName;
            SaveManager.Instance.Save(lastSavePointPosition, ZoneEnum.OldDunheim, "Scene0_0");
            SceneLoader.Instance.InitialLoad("Scene0_0");
        }
    
        private void OnPlayer_Entered(IOnPlayerEnteredEvent ev)
        {
            switch (ev.eventType)
            {
                case IOnPlayerEnteredEvent.EventType.CoinCollected:
                    // GainCoins(ev.numericData); // TODO? needed? Coins should be save only on save/death
                    // SceneDataHolder.Instance.AddData(ev.sceneName, ev.objectName); // TODO? needed? Coins should be roaming around scene, rather they should appear from chests/enemies etc. So we should save that chest was open, not that coin was taken
                    //CoinText.text = GameData.Coins.ToString(); TODO move responsibility of being updated to the component itself.
                    CoinsController.Instance.AddCoins(ev.numericData);              
                    ev.Remove();
                    break;
                case IOnPlayerEnteredEvent.EventType.DashCollected:
                    ev.Remove();
                    UnlockAbility(AbilityName.Dash);
                    SceneDataHolder.Instance.AddData(ev.sceneName, ev.objectName);
                    SaveManager.Instance.PersistObjectLoadingStrategy(ev.sceneName, ev.objectName);
                    break;
                case IOnPlayerEnteredEvent.EventType.DoubleJumpCollected:
                    ev.Remove();
                    UnlockAbility(AbilityName.DoubleJump);
                    SceneDataHolder.Instance.AddData(ev.sceneName, ev.objectName);
                    SaveManager.Instance.PersistObjectLoadingStrategy(ev.sceneName, ev.objectName);
                    break;
                case IOnPlayerEnteredEvent.EventType.WallJumpCollected:
                    ev.Remove();
                    UnlockAbility(AbilityName.WallJump);
                    SceneDataHolder.Instance.AddData(ev.sceneName, ev.objectName);
                    SaveManager.Instance.PersistObjectLoadingStrategy(ev.sceneName, ev.objectName);
                    break;
            }
        }

        private void Load(string directoryName, PreviewDataSchema previewDataSchema)
        {            
            this.directoryName = directoryName;
            lastSavePointPosition = new Vector2(previewDataSchema.savePointX, previewDataSchema.savePointY);
            // TODO Start loading animation here
            
            var data = SaveManager.Instance.Load(directoryName, previewDataSchema);
            AssignDataFromSave(data);
            SceneLoader.Instance.InitialLoad(data.previewDataSchema.sceneName);
            
            
            // TODO end loading animation here
            // TODO Start game entry animation, if one exists. Might be cool for simple and quick cinematics on game load.
            //GameObject.Find("Player").GetComponent<PlayerMovementController>().SetVariablesOnLoad(ref GameData); //move responsibility for data retrieval to Player
            Console.WriteLine("loaded");
        }
        

        public void UnlockAbility(AbilityName abilityName)
        {
            currentGameData.Abilities.AddOrUpdate(abilityName, true);
        }

        public bool IsAbilityUnlocked(AbilityName abilityName)
        {
            return currentGameData.Abilities.GetValueOrDefault(abilityName, false);
        }

        #region SAVE_DATA_ASSIGNMENT

        private void AssignDataFromSave(SaveDataSchema saveData)
        {
            currentGameData = saveData.gameData;
            AssignLoadDataToObjectLoadingStrategy();

            //LastSavePoint = saveData.SavePoint;
        }
        private void AssignLoadDataToObjectLoadingStrategy()
        {
            SceneDataHolder.Instance.SetLoadStrategyOnGameLoad(currentGameData.AlteredObjects);
        }
        #endregion
    }
}
