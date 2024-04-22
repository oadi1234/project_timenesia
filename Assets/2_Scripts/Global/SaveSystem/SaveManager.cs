using System;
using System.IO;
using System.Text;
using _2_Scripts.Global.SaveSystem.SaveDataSchemas;
using _2_Scripts.UI.Elements.HUD;
using Newtonsoft.Json;
using UnityEngine;

namespace _2_Scripts.Global.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance = null;
        private const string GameVersion = "0.1.0";
        private PreviewDataSchema _previewDataSchema;
        
        private GameData CurrentGameData => GameDataManager.Instance.currentGameData;

        public static string SavePath;
        public static string SavePreviewSuffix = "preview.dat";
        public static string SaveStateSuffix = "state.dat";

        private void Awake()
        {
            SavePath = $"{Application.persistentDataPath}/saves";
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            if (Instance == null)
            {
                Instance = this;
                Initialize();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Initialize()
        {
            Savepoint.OnSave += Save;
        }

        public void Save()
        {
            //Perform save using stored data.
            if (CurrentGameData == null)
            {
                Debug.Log(
                    "This literally should never happen."); //For unknown reason this still happens. Even though SaveData is assigned on load it still is seen as null.

                Save(new PreviewDataSchema(), new GameData(), GameDataManager.Instance.directoryName);
            }

            Save(_previewDataSchema, CurrentGameData, GameDataManager.Instance.directoryName);
        }

        public void Save(Vector2 position, ZoneEnum zone, string sceneName)
        {
            _previewDataSchema ??= new PreviewDataSchema();
            if (CurrentGameData == null)
                GameDataManager.Instance.PurgeGameData();
            
            SaveData(position, zone, sceneName);
            
            // AudioManager.PlaySound();
            //SoundManager.Instance.PlayOnce(GlobalAssets.Instance.SaveAudioClip); //disabled for testing.

            Save(_previewDataSchema, CurrentGameData, GameDataManager.Instance.directoryName);
        }

        private void SaveData(Vector2 position, ZoneEnum zone, string sceneName)
        {
            SaveGameData();
            SavePreviewData(position, zone, sceneName);
        }

        private void SaveGameData()
        {
            CurrentGameData.Coins = CoinsController.Instance != null ? CoinsController.Instance.Coins : 0;
        }

        private void SavePreviewData(Vector2 position, ZoneEnum zone, string sceneName)
        {
            _previewDataSchema.zone = zone;
            _previewDataSchema.sceneName = sceneName;
            _previewDataSchema.savePointX = position.x;
            _previewDataSchema.savePointY = position.y;
            // _saveData.previewDataSchema.gameVersion = gameVersion; //TODO
            _previewDataSchema.MaxEffort = CurrentGameData.MaxEffort;
            _previewDataSchema.MaxHealth = CurrentGameData.MaxHealth;
            _previewDataSchema.SpellCapacity = CurrentGameData.SpellCapacity;
        }

        public void PersistObjectLoadingStrategy(string sceneName, string objectName)
            // TODO I don't really like how this works.
            // the basic idea was to immediatelly save when a scene object has been permanently altered. It's a bit dumb though.
            // such save data should be done per scene if we want to keep it dynamically, to keep save sizes low (and loading times low)
            // and it gets stored in the state regardless.
            // Needs analysis. Might stay this way only because the game is relatively simple.
        {
            _previewDataSchema ??= new PreviewDataSchema();
            if (CurrentGameData == null)
                GameDataManager.Instance.PurgeGameData();

            CurrentGameData?.AlteredObjects.Add(sceneName + objectName, true);
            Save(_previewDataSchema, CurrentGameData, GameDataManager.Instance.directoryName); //TODO: preview save is not needed here, no? maybe depends
        }

        public SaveDataSchema Load(string directoryName = "save_0", PreviewDataSchema previewDataSchema = null)
        {
            var saveDataSchema = new SaveDataSchema
            {
                gameData = LoadData<GameData>($"{directoryName}_{SaveStateSuffix}", directoryName),
                previewDataSchema = previewDataSchema
            };

            return saveDataSchema;
        }

        public bool DeleteSave(string directoryName = "save_0")
        {
            var directory = $"{SavePath}/{directoryName}";
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
                return true;
            }

            return false;
        }

        public bool SaveExists(string directoryName = "save_0")
        {
            var directory = $"{SavePath}/{directoryName}";
            return Directory.Exists(directory);
        }

        public T LoadData<T>(string fileName, string directoryName = "save_0") //might explode.
        {
            if (!Directory.Exists($"{SavePath}/{directoryName}"))
            {
                throw new ArgumentException($"Could not find save directory: {directoryName}");
            }

            string filePath = $"{SavePath}/{directoryName}/{fileName}";
            if (File.Exists(filePath))
            {
                using (FileStream fs = File.Open($"{SavePath}/{directoryName}/{fileName}", FileMode.Open))
                {
                    BinaryReader br = new BinaryReader(fs);
                    byte[] bytes = Convert.FromBase64String(br.ReadString());
                    return (T) Activator.CreateInstance(typeof(T),
                        JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes)));
                }
            }
            else
            {
                throw new ArgumentException($"File {filePath} does not exist.");
            }
        }

        private bool SaveData<T>(T data, string fileName, string directoryName = "save_0")
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException(
                    $"Non-serializable data passed to SaveData method. Argument type: {typeof(T)}");
            }

            using var fs = File.Open($"{SavePath}/{directoryName}/{fileName}", FileMode.OpenOrCreate);
            var bw = new BinaryWriter(fs);
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            bw.Write(Convert.ToBase64String(bytes));
            bw.Flush();

            return true;
        }

        private bool Save(PreviewDataSchema previewDataSchema, GameData gameData, string directoryName = "save_0")
        {
            CheckSaveDirectory(directoryName);
            var result = false;
            if (previewDataSchema != null && gameData != null)
            {
                result = SaveData(previewDataSchema, $"{directoryName}_{SavePreviewSuffix}", directoryName)
                         && SaveData(gameData, $"{directoryName}_{SaveStateSuffix}", directoryName);
            }

            return result;
        }

        private void CheckSaveDirectory(string directoryName = "save_0")
        {
            if (!Directory.Exists($"{SavePath}/{directoryName}"))
            {
                Directory.CreateDirectory($"{SavePath}/{directoryName}");
            }
        }
    }
}
