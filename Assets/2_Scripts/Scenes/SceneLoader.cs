using _2___Scripts.Global;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private AsyncOperation loadingAction;
    private Dictionary<string, bool> loadedAreas = new Dictionary<string, bool>();
    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAreaGate.LoadArea += LoadArea;
            UnloadAreaGate.UnloadArea += UnloadArea;
            //LoadArea("Scene0_0");
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void InitialLoad(string sceneName)
    {
        loadingAction = SceneManager.LoadSceneAsync("PersistentScene", LoadSceneMode.Additive); //additive to ensure order of operation does not screw loading sequence.
        LoadArea(sceneName);
        loadingAction.completed += UnloadMainMenu;
    }
    public void LoadMainMenu()
    {
        loadingAction = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("PersistentScene");
        loadingAction.completed += CloseGameScenes;

    }

    private void CloseGameScenes(AsyncOperation obj)
    {
        foreach (var area in loadedAreas.Keys)
        {
            UnloadForMainMenu(area);
        }
        loadedAreas = new Dictionary<string, bool>();
    }

    private void UnloadMainMenu(AsyncOperation obj)
    {
        SceneManager.UnloadSceneAsync("MainMenu");
        //TODO send event that loading has completed here?
    }

    private void LoadArea(string sceneName)
    {
        if (!loadedAreas.GetValueOrDefault(sceneName, false))
        {
            loadedAreas[sceneName] = true; //if trying to load the same area then fail above condition
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        }
    }

    private void UnloadForMainMenu(string areaName)
    {
        if (loadedAreas.GetValueOrDefault(areaName, false))
        {
            SceneManager.UnloadSceneAsync(areaName);
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