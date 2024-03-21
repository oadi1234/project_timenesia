using System.Collections;
using UnityEngine;

/**
 * Class which should only take keyboard inputs and interpret them, calling relevant classes.
 * Should not contain any visual logic.
 */
public class UIInputManager : MonoBehaviour
{

    public IngameMenu menu;
    //public GameObject map;
    //public GameObject journal;
    //public GameObject inventory;
    //private bool _isMainMenuActive => mainMenu.activeSelf;
    // Start is called before the first frame update


    void Update()
    {
        if (Input.GetButtonDown("Cancel")) // TODO adjust for pad
        {
            menu.EscCommand();
        }
        else if (Input.GetButtonDown("Inventory")) {
            menu.OpenInventory();
        }
        //else if (!_isMainMenuActive)
        //{
        //    if (Input.GetKeyDown(KeyCode.M))
        //    {
        //        map.SetActive(!map.activeSelf);
        //        journal.SetActive(false);
        //        inventory.SetActive(false);
        //    }
        //    if (Input.GetKeyDown(KeyCode.J))
        //    {
        //        map.SetActive(false);
        //        journal.SetActive(!journal.activeSelf);
        //        inventory.SetActive(false);
        //    }
        //    if (Input.GetKeyDown(KeyCode.I))
        //    {
        //        map.SetActive(false);
        //        journal.SetActive(false);
        //        inventory.SetActive(!inventory.activeSelf);
        //    }
        //}
    }

    //private void DeactivatePanels()
    //{
    //    map.SetActive(false);
    //    journal.SetActive(false);
    //    inventory.SetActive(false);
    //}
}
