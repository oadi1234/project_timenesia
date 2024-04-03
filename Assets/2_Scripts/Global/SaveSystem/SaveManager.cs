using _2___Scripts.Global;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance = null;
	
    private void Awake()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
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
    
    private static SaveDataSchema saveData;

    private void Initialize()
    {
        Savepoint.OnSave += Save;
    }

    public void Save(string savepointCoordinates)
    {
        if (saveData == null)
            saveData = new SaveDataSchema();
        saveData.abilities = GameDataManager.Instance.stats.abilities;
        saveData.zone = ZoneEnum.None; //TODO assign from save point.

        // AudioManager.PlaySound();
        //SoundManager.Instance.PlayOnce(GlobalAssets.Instance.SaveAudioClip); //disabled for testing.

        Save(saveData);
    }

    public void PersistObjectLoadingStrategy(string sceneName, string objectName)
    {
        if (saveData == null)
            saveData = new SaveDataSchema();
        saveData.alteredObjects.Add(sceneName + objectName, true);
        Save(saveData);
    }

    public SaveDataSchema Load(string filename = "save_00")
    {
        SaveDataSchema data = new SaveDataSchema();
        string fullpath = GetPath(filename);

        if (File.Exists(fullpath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Open(fullpath, FileMode.Open))
            {
                data = (SaveDataSchema) bf.Deserialize(fs);
            }
        }

        return data;
    }

    private bool Save(SaveDataSchema data, string filename = "save_00")
    {

        bool result = false;
        string fullpath = GetPath(filename);
        if (data != null)
        {
            Debug.Log(data.Coins);
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Open(fullpath, FileMode.OpenOrCreate))
            {
                bf.Serialize(fs, data);
                result = true;
            }
        }

        return result;
    }

    public bool DeleteSave(string filename = "save_00")
    {
        string fullpath = GetPath(filename);
        if (File.Exists(fullpath))
        {
            File.Delete(fullpath);
            return true;
        }

        return false;
    }

    private string GetPath(string s)
    {
        return $"{Application.persistentDataPath}/saves/{s}.dat";
    }
}
