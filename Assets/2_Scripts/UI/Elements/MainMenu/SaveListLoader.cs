using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveListLoader : MonoBehaviour
{
    public SaveListManager SaveListManager;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(LoadListThenDeleteScript());
    }

    private IEnumerator LoadListThenDeleteScript()
    {
        var info = new DirectoryInfo($"{Application.persistentDataPath}/saves/").GetFiles();
        foreach ( var file in info)
        {
            if (file.Name.StartsWith("save_") && file.Name.EndsWith(".dat"))
            { 
                SaveListManager.saveFileNameList.Add(file.Name);
            }
            yield return null;
        }
        SaveListManager.GeneratePrefabList();
        // TODO In case we want to make the game load everything on start, we should send an event here that it is done and we can close the "loading intermission" screen.
        Destroy(this);
    }
}
