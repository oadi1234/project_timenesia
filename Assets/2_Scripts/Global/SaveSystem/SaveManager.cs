using _2___Scripts.Global;
using System;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance = null;

    public static string SavePath;
    public static string SavePreviewSuffix = "preview.dat";
    public static string SaveStateSuffix = "state.dat";
	
    private void Awake()
    {
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
        SavePath = $"{Application.persistentDataPath}/saves";
    }
    
    private static SaveDataSchema saveData;

    private void Initialize()
    {
        Savepoint.OnSave += Save;
    }

    public void Save(Vector2 position, ZoneEnum zone, string sceneName)
    {
        if (saveData == null)
        {
            saveData = new SaveDataSchema(new PreviewStatsDataSchema(), new GameStateSaveDataSchema());
        }
        saveData.gameStateSaveDataSchema.abilities = GameDataManager.Instance.stats.abilities;
        saveData.previewStatsDataSchema.zone = zone;
        saveData.previewStatsDataSchema.sceneName = sceneName;
        saveData.previewStatsDataSchema.savePointX = position.x;
        saveData.previewStatsDataSchema.savePointY = position.y;

        // AudioManager.PlaySound();
        //SoundManager.Instance.PlayOnce(GlobalAssets.Instance.SaveAudioClip); //disabled for testing.

        Save(saveData);
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
            saveData = new SaveDataSchema(new PreviewStatsDataSchema(), new GameStateSaveDataSchema());
        }
        saveData.gameStateSaveDataSchema.alteredObjects.Add(sceneName + objectName, true);
        Save(saveData);
    }

    public SaveDataSchema Load(string directoryName = "save_00")
    {
        SaveDataSchema data = new SaveDataSchema(
            LoadData<PreviewStatsDataSchema>($"{directoryName}_{SavePreviewSuffix}", directoryName),
            LoadData<GameStateSaveDataSchema>($"{directoryName}_{SaveStateSuffix}", directoryName));

        return data;
    }
    public bool DeleteSave(string directoryName = "save_00")
    {
        string fullpath = GetPath(directoryName);
        if (File.Exists(fullpath))
        {
            File.Delete(fullpath);
            return true;
        }

        return false;
    }

    private bool Save(SaveDataSchema data, string directoryName = "save_00")
    {
        bool result = false;
        if (data != null)
        {
            result = SaveData(data.previewStatsDataSchema, $"{directoryName}_preview.dat", directoryName)
                && SaveData(data.gameStateSaveDataSchema, $"{directoryName}_state.dat", directoryName);
        }

        return result;
    }

    private bool SaveData<T>(T data, string fileName, string directoryName = "save_00")
    {
        if (typeof(T).IsSerializable)
        {
            throw new ArgumentException($"Non-serializable data passed to SaveData method. Argument type: {typeof(T)}");
        }
        using (FileStream fs = File.Open($"{SavePath}/{directoryName}/{fileName}.dat", FileMode.OpenOrCreate))
        {
            BinaryWriter bw = new BinaryWriter(fs);
            byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            bw.Write(Convert.ToBase64String(bytes));
            bw.Flush();
        }
        return true;
    }

    public T LoadData<T>(string fileName, string directoryName = "save_00") //might explode.
    {
        if (!Directory.Exists($"{SavePath}/{directoryName}"))
        {
            throw new ArgumentException($"Could not find save directory: {directoryName}");
        }
        string filePath = $"{SavePath}/{directoryName}/{fileName}.dat";
        if (File.Exists(filePath))
        {
            using (FileStream fs = File.Open($"{SavePath}/{directoryName}/{fileName}.dat", FileMode.Open))
            {
                BinaryReader br = new BinaryReader(fs);
                byte[] bytes = Convert.FromBase64String(br.ReadString());
                return (T)Activator.CreateInstance(typeof(T), JsonUtility.FromJson<T>(Encoding.UTF8.GetString(bytes)));
            }
        }
        else
        {
            throw new ArgumentException($"File {filePath} does not exist.");
        }
    }


    private string GetPath(string s)
    {
        return $"{SavePath}/{s}.dat";
    }
}
