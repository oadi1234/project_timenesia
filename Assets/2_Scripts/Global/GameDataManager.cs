using System;
using System.Collections.Generic;
using _2___Scripts.Global.Events;
using _2_Scripts.ExtensionMethods;
using _2_Scripts.Global.Events;
using _2_Scripts.Global.Events.Model;
using _2_Scripts.Global.SaveSystem;
using _2_Scripts.Global.SaveSystem.SaveDataSchemas;
using _2_Scripts.Player;
using _2_Scripts.Player.model;
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
            Spellbook.Instance.PopulateAllSpells();

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
            currentGameData.MaxEffort = 4; //TODO set to 2, it's just better for testing.
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
            switch (ev.collectedEventType)
            {
                case CollectedEventType.CoinCollected:
                    CoinsController.Instance.AddCoins(ev.numericData);     
                    break;
                case CollectedEventType.DashCollected:
                case CollectedEventType.DoubleJumpCollected:
                case CollectedEventType.WallJumpCollected:
                case CollectedEventType.LongDashCollected:
                case CollectedEventType.SpatialDashCollected:
                case CollectedEventType.TimeGateCollected:
                case CollectedEventType.SwimUnderwaterCollected:
                case CollectedEventType.SlowmotionFocusCollected:
                case CollectedEventType.MidAirFocusCollected:
                case CollectedEventType.ChargeSpellCollected:
                    UnlockAbility(Mappers.Map(ev.collectedEventType));
                    SceneDataHolder.Instance.AddData(ev.sceneName, ev.objectName);
                    SaveManager.Instance.PersistObjectLoadingStrategy(ev.sceneName, ev.objectName);
                    break;
                case CollectedEventType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }         
            ev.Remove();
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

        private void UnlockAbility(UnlockableName unlockableName)
        {
            currentGameData.Abilities.AddOrUpdate(unlockableName, true);
        }

        public bool IsAbilityUnlocked(UnlockableName unlockableName)
        {
            return currentGameData.Abilities.GetValueOrDefault(unlockableName, false);
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
