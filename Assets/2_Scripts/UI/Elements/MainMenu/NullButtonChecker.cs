using _2_Scripts.UI.Elements;
using _2_Scripts.UI.Elements.MainMenu;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Class designed for UIPanel with modifiable button list.
// All this does is it stalls for a bit until at least one button gets detected as null
// Then it calls the "RemoveNullButtons()" method from UI Panel.
// Used because Unity waits till end of the next frame to actually remove an element.
// 
public class NullButtonChecker : MonoBehaviour
{
    public MainMenuManager mainMenuManager;

    public IEnumerator StallUntilNullIsFound()
    {
        UIPanel saveListPanel = GetComponent<UIPanel>();
        while (!saveListPanel.buttons.Any(button => button==null)) 
        {
            yield return null;
        }
        saveListPanel.RemoveNullButtons();
        mainMenuManager.OpenSavedGamesAfterDelete();
    }
}
