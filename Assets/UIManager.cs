using Newtonsoft.Json.Serialization;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject map;
    public GameObject journal;
    public GameObject inventory;
    private bool _isMainMenuActive => mainMenu.activeSelf;
    private bool _isAnyNonMainMenuPanelActive => map.activeSelf || journal.activeSelf || inventory.activeSelf;
    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(false);
        map.SetActive(false);
        journal.SetActive(false);
        inventory.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isAnyNonMainMenuPanelActive)
                DeactivatePanels();
            else 
                mainMenu.SetActive(!mainMenu.activeSelf);
        }
        else if (!_isMainMenuActive)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                map.SetActive(!map.activeSelf);
                journal.SetActive(false);
                inventory.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                map.SetActive(false);
                journal.SetActive(!journal.activeSelf);
                inventory.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                map.SetActive(false);
                journal.SetActive(false);
                inventory.SetActive(!inventory.activeSelf);
            }
        }
    }

    private void DeactivatePanels()
    {
        map.SetActive(false);
        journal.SetActive(false);
        inventory.SetActive(false);
    }
}
