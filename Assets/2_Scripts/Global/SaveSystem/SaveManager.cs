using System;
using System.IO;
using System.Text;
using _2_Scripts.Global.SaveSystem.SaveDataSchemas;
using Newtonsoft.Json;
using UnityEngine;

namespace _2_Scripts.Global.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance = null;
        private static int gameVersion = 0;

        private SaveDataSchema saveData;

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
            DontDestroyOnLoad (gameObject);
        }

        private void Initialize()
        {
            Savepoint.OnSave += Save;
        }

        public void Save()
        {
            //Perform save using stored data.
            if (saveData == null)
            {
                Debug.Log("This literally should never happen."); //For unknown reason this still happens. Even though SaveData is assigned on load it still is seen as null.
                saveData = new SaveDataSchema();
                saveData.previewStatsDataSchema = new PreviewStatsDataSchema();
                saveData.gameStateSaveDataSchema = new GameStateSaveDataSchema();
            }
            Save(saveData, GameDataManager.Instance.directoryName);

        }

        public void Save(Vector2 position, ZoneEnum zone, string sceneName)
        {
            if (saveData == null)
            {
                saveData = new SaveDataSchema();
                saveData.previewStatsDataSchema = new PreviewStatsDataSchema();
                saveData.gameStateSaveDataSchema = new GameStateSaveDataSchema();
            }
            saveData.gameStateSaveDataSchema.abilities = GameDataManager.Instance.stats.abilities;
            saveData.previewStatsDataSchema.zone = zone;
            saveData.previewStatsDataSchema.sceneName = sceneName;
            saveData.previewStatsDataSchema.savePointX = position.x;
            saveData.previewStatsDataSchema.savePointY = position.y;
            saveData.previewStatsDataSchema.MaxEffort = GameDataManager.Instance.stats.MaxEffort;
            saveData.previewStatsDataSchema.MaxHealth = GameDataManager.Instance.stats.MaxHealth;
            saveData.previewStatsDataSchema.gameVersion = gameVersion;
            saveData.gameStateSaveDataSchema.Coins = GameDataManager.Instance.stats.Coins;
            saveData.previewStatsDataSchema.SpellCapacity = GameDataManager.Instance.stats.SpellCapacity;

            // AudioManager.PlaySound();
            //SoundManager.Instance.PlayOnce(GlobalAssets.Instance.SaveAudioClip); //disabled for testing.

            Save(saveData, GameDataManager.Instance.directoryName);
        }

        public void PersistObjectLoadingStrategy(string sceneName, string objectName) 
            // TODO I don't really like how this works.
            // the basic idea was to immediatelly save when a scene object has been permanently altered. It's a bit dumb though.
            // such save data should be done per scene if we want to keep it dynamically, to keep save sizes low (and loading times low)
            // and it gets stored in the state regardless.
            // Needs analysis. Might stay this way only because the game is relatively simple.
        {
            if (saveData == null)
            {
                Debug.Log("This literally should never happen.");
                saveData = new SaveDataSchema();
                saveData.previewStatsDataSchema = new PreviewStatsDataSchema();
                saveData.gameStateSaveDataSchema = new GameStateSaveDataSchema();
            }
            saveData.gameStateSaveDataSchema.alteredObjects.Add(sceneName + objectName, true);
            Save(saveData, GameDataManager.Instance.directoryName);
        }



        public SaveDataSchema Load(string directoryName = "save_0")
        {
            SaveDataSchema data = new SaveDataSchema();
            data.previewStatsDataSchema = LoadData<PreviewStatsDataSchema>($"{directoryName}_{SavePreviewSuffix}", directoryName);
            data.gameStateSaveDataSchema = LoadData<GameStateSaveDataSchema>($"{directoryName}_{SaveStateSuffix}", directoryName);

            saveData = data;

            return data;
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

        private bool Save(SaveDataSchema data, string directoryName = "save_0")
        {
            CreateSaveDirectory(directoryName);
            bool result = false;
            if (data != null)
            {
                result = SaveData(data.previewStatsDataSchema, $"{directoryName}_{SavePreviewSuffix}", directoryName)
                         && SaveData(data.gameStateSaveDataSchema, $"{directoryName}_{SaveStateSuffix}", directoryName);
            }

            return result;
        }

        private void CreateSaveDirectory(string directoryName = "save_0")
        {
            if (!Directory.Exists($"{SavePath}/{directoryName}"))
            {
                Directory.CreateDirectory($"{SavePath}/{directoryName}");
            }
        }

        private bool SaveData<T>(T data, string fileName, string directoryName = "save_0")
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException($"Non-serializable data passed to SaveData method. Argument type: {typeof(T)}");
            }
            using (FileStream fs = File.Open($"{SavePath}/{directoryName}/{fileName}", FileMode.OpenOrCreate))
            {
                BinaryWriter bw = new BinaryWriter(fs);
                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
                bw.Write(Convert.ToBase64String(bytes));
                bw.Flush();
            }
            return true;
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
                    return (T)Activator.CreateInstance(typeof(T), JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes)));
                }
            }
            else
            {
                throw new ArgumentException($"File {filePath} does not exist.");
            }
        }
    }
}
