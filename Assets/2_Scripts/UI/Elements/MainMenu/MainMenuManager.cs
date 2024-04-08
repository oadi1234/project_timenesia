using System;
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
    public UIPanel gameScrollListPanel;
    public UIPanel exitConfirmationPanel;
    public UIPanel extrasPanel;

    private UIMainMenuWindowType currentlyOpen = UIMainMenuWindowType.MainMenu;

    //public static event Action OnMenuPrepared;

    private void Awake()
    {
        menuPanel.SelectButton(0);
        //OnMenuPrepared();
        LoadButton.DeleteAction += OpenDeleteSaveConfirmationPanel;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) // TODO adjust for pad
        {
            EscCommand();
        }
    }

    public void StartButton()
    {
        //Do one of two things:
        // If the game has no saves created yet - create empty save and start the game
        // else open saved games panel.
        if(SaveListManager.saveDirectoryNameList.Count == 0)
        {
            global::StartButton.StartNewGame();
        }
        else
        {
            OpenSavedGamesPanel();
        }
    }

    public void OpenOptionsPanel()
    {
        currentlyOpen = UIMainMenuWindowType.Options;
        menuPanel.ToggleActive();
        optionsPanel.ToggleActive();
        optionsPanel.SelectButton(0);
    }

    public void CloseOptionsPanel()
    {
        currentlyOpen = UIMainMenuWindowType.MainMenu;
        optionsPanel.ToggleActive();
        menuPanel.ToggleActive();
        menuPanel.SelectButton(0);
    }

    public void OpenCreditsPanel()
    {
        currentlyOpen = UIMainMenuWindowType.Credits;
        menuPanel.ToggleActive();
        creditsPanel.ToggleActive();
    }

    public void CloseCreditsPanel()
    {
        currentlyOpen = UIMainMenuWindowType.MainMenu;
        creditsPanel.ToggleActive();
        menuPanel.ToggleActive();
        menuPanel.SelectButton(0);
    }

    public void OpenExitConfirmationPanel()
    {
        currentlyOpen = UIMainMenuWindowType.ExitPanel;
        menuPanel.ToggleActive();
        exitConfirmationPanel.ToggleActive();
        exitConfirmationPanel.SelectButton(0);
    }

    public void CloseExitConfirmationPanel()
    {
        currentlyOpen = UIMainMenuWindowType.MainMenu;
        exitConfirmationPanel.ToggleActive();
        menuPanel.ToggleActive();
        menuPanel.SelectButton(0);
    }

    public void OpenSavedGamesPanel()
    {
        currentlyOpen = UIMainMenuWindowType.LoadGame;
        menuPanel.ToggleActive();
        savedGamesPanel.ToggleActive();
        gameScrollListPanel.ToggleActive();
        gameScrollListPanel.SelectButton(0);
    }

    public void CloseSavedGamesPanel()
    {
        currentlyOpen = UIMainMenuWindowType.MainMenu;
        savedGamesPanel.ToggleActive();
        gameScrollListPanel.ToggleActive();
        menuPanel.ToggleActive();
        menuPanel.SelectButton(0);
    }

    public void OpenDeleteSaveConfirmationPanel(string directoryName)
    {
        currentlyOpen = UIMainMenuWindowType.DeleteSave;
        deleteSaveConfirmationPanel.ToggleActive();
        deleteSaveConfirmationPanel.GetComponent<DeleteSavePanelScript>().directoryName = directoryName;
        savedGamesPanel.ToggleActive();
        gameScrollListPanel.ToggleActive();
        deleteSaveConfirmationPanel.SelectButton(0);
    }

    public void CloseDeleteSaveConfirmationPanel()
    {
        currentlyOpen = UIMainMenuWindowType.LoadGame;
        deleteSaveConfirmationPanel.ToggleActive();
        savedGamesPanel.ToggleActive();
        gameScrollListPanel.ToggleActive();
        gameScrollListPanel.SelectButton(0);
    }

    public void OpenAchievementsPanel()
    {
        currentlyOpen = UIMainMenuWindowType.Achievements;
        menuPanel.ToggleActive();
        achievementsPanel.ToggleActive();
        achievementsPanel.SelectButton(0);
    }

    public void CloseAchievementsPanel()
    {
        currentlyOpen = UIMainMenuWindowType.MainMenu;
        achievementsPanel.ToggleActive();
        menuPanel.ToggleActive();
        menuPanel.SelectButton(0);
    }

    public void OpenExtrasPanel()
    {
        currentlyOpen = UIMainMenuWindowType.Extras;
        menuPanel.ToggleActive();
        extrasPanel.ToggleActive();
        extrasPanel.SelectButton(0);
    }

    public void CloseExtrasPanel()
    {
        currentlyOpen = UIMainMenuWindowType.MainMenu;
        extrasPanel.ToggleActive();
        menuPanel.ToggleActive();
        menuPanel.SelectButton(0);
    }

    public void EscCommand()
    {
        switch (currentlyOpen)
        {
            case UIMainMenuWindowType.None:
                // TODO as below - fade main menu back in?
                break;
            case UIMainMenuWindowType.MainMenu:
                // TODO might actually add option to fade out main menu to get a better view of the background.
                break;
            case UIMainMenuWindowType.ExitPanel:
                CloseExitConfirmationPanel();
                break;
            case UIMainMenuWindowType.LoadGame:
                CloseSavedGamesPanel();
                break;
            case UIMainMenuWindowType.Options:
                CloseOptionsPanel();
                break;
            case UIMainMenuWindowType.Extras:
                CloseExtrasPanel();
                break;
            case UIMainMenuWindowType.Achievements:
                CloseAchievementsPanel();
                break;
            case UIMainMenuWindowType.Credits:
                CloseCreditsPanel();
                break;
            case UIMainMenuWindowType.DeleteSave:
                CloseDeleteSaveConfirmationPanel();
                break;
            default:
                break;
        }
    }
    public void Quit()
    {
        // Quit Game
        Application.Quit();
        Debug.Log("Application.Quit() calls are ignored in the editor.");
    }

    public void DetachListener()
    {
        LoadButton.DeleteAction -= OpenDeleteSaveConfirmationPanel;
    }
}
