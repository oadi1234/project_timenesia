using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveListManager : MonoBehaviour
{
    public List<string> saveFileNameList = new List<string>();

    public GameObject newGameListElement;
    public GameObject loadGameListElement;
    public RectTransform ScrollList;

    private List<GameObject> games = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GeneratePrefabList()
    {

    }
}
