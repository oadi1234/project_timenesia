using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private AsyncOperation loadingAction;
    private Dictionary<string, bool> loadedAreas = new Dictionary<string, bool>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadAreaGate.LoadArea += LoadArea;
        UnloadAreaGate.UnloadArea += UnloadArea;
        //LoadArea("Scene0_0");
    }

    public void InitialLoad(string areaName)
    {
        loadingAction = SceneManager.LoadSceneAsync("PersistentScene", LoadSceneMode.Additive); //additive to ensure order of operation does not screw loading sequence.
        LoadArea(areaName);
        loadingAction.completed += UnloadMainMenu;
    }

    public void CloseGameScenes(AsyncOperation obj)
    {
        foreach (var area in loadedAreas.Keys)
        {
            UnloadArea(area);
        }
    }

    private void LoadMainMenu()
    {
        loadingAction = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("PersistentScene");
        loadingAction.completed += CloseGameScenes;

    }

    private void UnloadMainMenu(AsyncOperation obj)
    {
        SceneManager.UnloadSceneAsync("MainMenu");
        //TODO send event that loading has completed here?
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