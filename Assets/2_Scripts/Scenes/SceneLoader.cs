using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public SceneDataHolder sceneDataHolder;
    private bool loaded = false;

    private Dictionary<string, bool> loadedAreas = new Dictionary<string, bool>();
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadAreaGate.LoadArea += LoadArea;
        UnloadAreaGate.UnloadArea += UnloadArea;
        if (!loaded)
        {
            loaded = true;
            SceneManager.LoadScene("Scene0_0", LoadSceneMode.Additive);
            loadedAreas["Scene0_0"] = true;
        }
    }

    private void LoadArea(string areaName)
    {
        if (!loadedAreas.GetValueOrDefault(areaName, false))
        {
            loadedAreas[areaName] = true; //if trying to load the same area then fail above condition
            var loadOperation = SceneManager.LoadSceneAsync(areaName, LoadSceneMode.Additive);
            RemoveCollectedItems(areaName, loadOperation);
            AlterCollectedSavePersistent(areaName, loadOperation);

        }
    }

    private void RemoveCollectedItems(string areaName, AsyncOperation operation)
    {
        List<string> deactivateObjects = sceneDataHolder.tryGetPerLoadBehaviour(areaName);
        operation.completed += (x) =>
        {
            if (deactivateObjects != null)
            {
                StartCoroutine(DeactivateObjects(areaName, sceneDataHolder.tryGetPerLoadBehaviour(areaName)));

            }
        };
    }

    private void AlterCollectedSavePersistent(string areaName, AsyncOperation operation)
    {
        //Logic here will most likely require more adjustments - abilities will likely require different save_persistence behaviour than open chests.
        // TODO it is now mostly left like this for testing behaviour per 2 different types of object, but it will require adjustments.
        // It might even be better to set it up all in a single coroutine, which has different method calls depending on the type of object?
        List<string> alteredObjects = sceneDataHolder.tryGetPerLoadBehaviour(areaName, LoadObjectBehaviour.SAVE_PERSIST);
        operation.completed += (x) =>
        {
            if (alteredObjects != null)
            {
                StartCoroutine(DeactivateObjects(areaName, sceneDataHolder.tryGetPerLoadBehaviour(areaName, LoadObjectBehaviour.SAVE_PERSIST)));
            }
        };
    }

    private void UnloadArea(string areaName)
    {
        if (loadedAreas.GetValueOrDefault(areaName, false))
        {
            loadedAreas.Remove(areaName);
            SceneManager.UnloadSceneAsync(areaName);
        }
    }

    private IEnumerator DeactivateObjects(string areaName, List<string> names)
    {
        foreach (string name in names)
        {
            Debug.Log(name);
            yield return null;
            GameObject.Find(name).SetActive(false);
        }
    }

    internal void SetSceneDataHolder(ref SceneDataHolder data)
    {
        sceneDataHolder = data;
    }
}