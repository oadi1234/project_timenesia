using _2_Scripts.UI.Elements.InGame;
using UnityEngine;

namespace _2_Scripts.UI
{
    /**
 * Class which should only take keyboard inputs and interpret them, calling relevant classes.
 * Should not contain any visual logic.
 */
    public class UIInputManager : MonoBehaviour
    {

        public MenuManager menuManager;


        void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                menuManager.EscCommand();
            }
            else if (Input.GetButtonDown("Inventory")) {
                menuManager.ToggleInventory();
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
}
