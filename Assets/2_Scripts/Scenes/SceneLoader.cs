using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    private Dictionary<string, bool> loadedAreas = new Dictionary<string, bool>();
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadAreaGate.LoadArea += LoadArea;
        UnloadAreaGate.UnloadArea += UnloadArea;
        LoadArea("Scene0_0");
    }

    private void LoadArea(string areaName)
    {
        if (!loadedAreas.GetValueOrDefault(areaName, false))
        {
            loadedAreas[areaName] = true; //if trying to load the same area then fail above condition
            SceneManager.LoadSceneAsync(areaName, LoadSceneMode.Additive);
        }
    }

    private void UnloadArea(string areaName)
    {
        if (loadedAreas.GetValueOrDefault(areaName, false))
        {
            loadedAreas.Remove(areaName);
            SceneManager.UnloadSceneAsync(areaName);
        }
    }
}