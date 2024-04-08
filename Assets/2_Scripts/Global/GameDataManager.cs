using System;
using _2___Scripts.Global.Events;
using _2_Scripts.Global.Events;
using _2_Scripts.Global.SaveSystem;
using _2_Scripts.Global.SaveSystem.Player;
using _2_Scripts.Global.SaveSystem.SaveDataSchemas;
using _2_Scripts.Scenes;
using _2_Scripts.UI.Elements.MainMenu;
using UnityEngine;

namespace _2_Scripts.Global
{
    public class GameDataManager : MonoBehaviour
    {
        private GameDataManager()
        {
        }
        public PlayerAbilityAndStats stats;

        public static GameDataManager Instance { get; private set; }
        public static event Action<IBaseEvent> OnCollected;

        public Vector2 LastSavePointPosition;
        public Savepoint lastSavepoint;
        public string directoryName;


        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;

                stats = new PlayerAbilityAndStats();
                LastSavePointPosition = Vector2.zero;
                DontDestroyOnLoad(gameObject);
                OnPlayerEnteredEvent.OnPlayerEntered += OnPlayer_Entered;
                LoadButton.LoadAction += Load;
                StartButton.NewGame += SetGameStateForNewGame;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            //GameObject.Find("Player").GetComponent<PlayerMovementController>().SetVariablesOnLoad(ref stats); //move responsibility to load stats to player
            //CoinText = GameObject.Find("CoinCounter").GetComponent<TextMeshProUGUI>(); // see above

        }

        private void SetGameStateForNewGame(string directoryName)
        {
            PurgeStats();
            SetStatsForNewGameStart();
            this.directoryName = directoryName;
            SaveManager.Instance.Save(LastSavePointPosition, ZoneEnum.OldDunheim, "Scene0_0");
            SceneLoader.Instance.InitialLoad("Scene0_0");
        }
    
        private void OnPlayer_Entered(IOnPlayerEnteredEvent obj)
        {
            switch (obj.eventType)
            {
                case IOnPlayerEnteredEvent.EventType.CoinCollected:
                    GainCoins(obj.numericData);
                    obj.Remove();
                    SceneDataHolder.Instance.AddData(obj.sceneName, obj.objectName);
                    //CoinText.text = stats.Coins.ToString(); TODO move responsibility of being updated to the component itself.
                    break;
                case IOnPlayerEnteredEvent.EventType.DashCollected:
                    obj.Remove();
                    stats.UnlockAbility(AbilityName.Dash);
                    SceneDataHolder.Instance.AddData(obj.sceneName, obj.objectName);
                    SaveManager.Instance.PersistObjectLoadingStrategy(obj.sceneName, obj.objectName);
                    break;
                case IOnPlayerEnteredEvent.EventType.DoubleJumpCollected:
                    obj.Remove();
                    stats.UnlockAbility(AbilityName.DoubleJump);
                    SceneDataHolder.Instance.AddData(obj.sceneName, obj.objectName);
                    SaveManager.Instance.PersistObjectLoadingStrategy(obj.sceneName, obj.objectName);
                    break;
                case IOnPlayerEnteredEvent.EventType.WallJumpCollected:
                    obj.Remove();
                    stats.UnlockAbility(AbilityName.WallJump);
                    SceneDataHolder.Instance.AddData(obj.sceneName, obj.objectName);
                    SaveManager.Instance.PersistObjectLoadingStrategy(obj.sceneName, obj.objectName);
                    break;
            }
        }

        private void Load(string directoryName)
        {
            // TODO Start loading animation here
            SaveDataSchema data = SaveManager.Instance.Load(directoryName);
            AssignDataFromSave(data);
            SceneLoader.Instance.InitialLoad(data.previewStatsDataSchema.sceneName);
            LastSavePointPosition = new Vector2(data.previewStatsDataSchema.savePointX, data.previewStatsDataSchema.savePointY);
            // TODO end loading animation here
            // TODO Start game entry animation, if one exists. Might be cool for simple and quick cinematics on game load.
            //GameObject.Find("Player").GetComponent<PlayerMovementController>().SetVariablesOnLoad(ref stats); //move responsibility for data retrieval to Player
            this.directoryName = directoryName;
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
            //CoinText.text = stats.Coins.ToString(); TODO move responsibility of being updated to the component itself.
        }

        private void AssignLoadDataToAbilities(SaveDataSchema saveData)
        {
            stats.abilities = saveData.gameStateSaveDataSchema.abilities;
        }

        private void AssignLoadDataToObjectLoadingStrategy(SaveDataSchema saveData)
        {
            SceneDataHolder.Instance.SetLoadStrategyOnGameLoad(saveData.gameStateSaveDataSchema.alteredObjects);
        }
        #endregion

        public bool TakeDamage(int amount) // TODO I have a feeling this shouldn't be here.
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
