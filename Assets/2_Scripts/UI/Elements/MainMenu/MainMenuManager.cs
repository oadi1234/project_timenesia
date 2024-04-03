using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * TODO set as the script for the actual main menu.
 */
public class MainMenuManager : MonoBehaviour
{
    public SaveListManager SaveListManager;
    public UIPanel optionsPanel;
    public UIPanel deleteSaveConfirmationPanel;
    public UIPanel savedGamesPanel;
    public UIPanel creditsPanel;
    public UIPanel achievementsPanel;
    public UIPanel menuPanel;

    private UIMainMenuWindowType currentlyOpen = UIMainMenuWindowType.MainMenu;

    private void Awake()
    {
        menuPanel.SelectButton(0);
    }

    public void StartButton()
    {
        //Do one of two things:
        // If the game has no saves created yet - create empty save and start the game
        // else open saved games panel.
        if(SaveListManager.saveFileNameList.Count == 0)
        {
            StartAction.StartNewGame();
        }
        else
        {
            currentlyOpen = UIMainMenuWindowType.LoadGame;
            menuPanel.ToggleActive();
            savedGamesPanel.ToggleActive();
        }
    }

    public void CloseSavedGamesList()
    {

    }

    public void OptionsButton()
    {
        currentlyOpen = UIMainMenuWindowType.Options;
    }

    public void CreditsButton()
    {
        // Show Credits Menu
    }

    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
        Debug.Log("Application.Quit() calls are ignored in the editor.");
    }

    public void EscCommand()
    {
        switch (currentlyOpen)
        {
            case UIMainMenuWindowType.None:
                break;
            case UIMainMenuWindowType.MainMenu:
                break;
            case UIMainMenuWindowType.ExitPanel:
                break;
            case UIMainMenuWindowType.LoadGame:
                break;
            case UIMainMenuWindowType.Options:
                break;
            case UIMainMenuWindowType.Extras:
                break;
            case UIMainMenuWindowType.Achievements:
                break;
            case UIMainMenuWindowType.Credits:
                break;
            default:
                break;
        }
    }
}
