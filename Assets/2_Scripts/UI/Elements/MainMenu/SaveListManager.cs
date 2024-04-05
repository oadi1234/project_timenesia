using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveListManager : MonoBehaviour
{
    public List<string> saveDirectoryNameList = new List<string>();

    public GameObject newGameListElement;
    public GameObject loadGameListElement;
    public RectTransform ScrollList;

    private List<GameObject> games = new List<GameObject>();

    private void Awake()
    {
        MainMenuManager.OnMenuPrepared += GeneratePrefabList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GeneratePrefabList()
    {
        foreach (string directoryName in saveDirectoryNameList) 
        {
            PreviewStatsDataSchema schema = SaveManager.Instance.LoadData<PreviewStatsDataSchema>($"{directoryName}_{SaveManager.SavePreviewSuffix}", directoryName);
            
        }
    }
}
