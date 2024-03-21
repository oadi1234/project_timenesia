using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenu : MonoBehaviour
{
    public MenuPanel menuPanel;
    public FadeController dimValueSetter;
    public FadeController backgroundDimValueSetter;
    public FadeController menuPanelFade;
    public FadeController exitConfirmationPanelFade;
    public FadeController mainMenuConfirmationPanelFade;

    public bool isMenuActive { get { return menuPanel.gameObject.activeSelf; } }
    private bool canOpenInventory = true;
    private IEnumerator fadeInCoroutine;
    private IEnumerator fadeOutCoroutine;
    private IEnumerator bgFadeInCoroutine;
    private IEnumerator bgFadeOutCoroutine;
    private UIWindowType currentlyOpenMenu = UIWindowType.None;
    private UIWindowType lastMenuOpened = UIWindowType.Inventory; //QoL for game pad users - "inventory key" will open the last browsed menu.
    //public GameObject OptionsMenu;

    private void Awake()
    {
        fadeOutCoroutine = dimValueSetter.DoFadeOut();
        bgFadeOutCoroutine = backgroundDimValueSetter.DoFadeOut();
        fadeInCoroutine = dimValueSetter.DoFadeIn();
        bgFadeInCoroutine = backgroundDimValueSetter.DoFadeIn();
    }

    public void EscCommand()
    {
        switch(currentlyOpenMenu)
        {
            case UIWindowType.None:
                canOpenInventory = false;
                OpenMenu();
                break;
            case UIWindowType.ExitPanel:
                CloseExitGameConfirmationMenu();
                break;
            case UIWindowType.InGameMenu:
                canOpenInventory = true;
                CloseMenu();
                break;
            case UIWindowType.MainMenuPanel:
                CloseMainMenuConfirmationMenu();
                break;
            case UIWindowType.Inventory:
            case UIWindowType.Map:
            case UIWindowType.Spells:
            case UIWindowType.Bestiary:
                ClosePlayerMenu();
                break;
            default:
                break;
        }
    }

    public void CloseMenu()
    {
        currentlyOpenMenu = UIWindowType.None;
        menuPanel.ToggleActive();
        DoFadeOut();
    }

    public void OpenMenu()
    {
        currentlyOpenMenu = UIWindowType.InGameMenu;
        menuPanel.ToggleActive();
        DoFadeIn();
    }

    public void OpenOptionsSubmenu()
    {
        Debug.Log("Options Menu");
    }

    public void CloseOptionsSubmenu()
    {

    }

    public void GoToMainMenu()
    {
        Debug.Log("Will save game, unload the scene and put player back in main menu.");
    }

    public void OpenExitGameConfirmationMenu()
    {
        //Fade buttons out
        currentlyOpenMenu = UIWindowType.ExitPanel;
        FadeOutPanelAndDisableIt(menuPanelFade.DoFadeOut(), menuPanel.gameObject);
        FadeInPanelAndEnableIt(exitConfirmationPanelFade.DoFadeIn(), exitConfirmationPanelFade.gameObject);
        //Fade in exit confirmation after buttons are faded out
    }

    public void CloseExitGameConfirmationMenu()
    {
        currentlyOpenMenu = UIWindowType.InGameMenu;
        FadeOutPanelAndDisableIt(exitConfirmationPanelFade.DoFadeOut(), exitConfirmationPanelFade.gameObject);
        FadeInPanelAndEnableIt(menuPanelFade.DoFadeIn(), menuPanel.gameObject);
        //fade out exit confirmation
        //fade in menu buttons after exit confirmation is faded out
    }

    public void OpenMainMenuConfirmationMenu()
    {
        //Fade buttons out
        currentlyOpenMenu = UIWindowType.MainMenuPanel;
        FadeOutPanelAndDisableIt(menuPanelFade.DoFadeOut(), menuPanel.gameObject);
        FadeInPanelAndEnableIt(mainMenuConfirmationPanelFade.DoFadeIn(), mainMenuConfirmationPanelFade.gameObject);
        //Fade in exit confirmation after buttons are faded out
    }

    public void CloseMainMenuConfirmationMenu()
    {
        currentlyOpenMenu = UIWindowType.InGameMenu;
        FadeOutPanelAndDisableIt(mainMenuConfirmationPanelFade.DoFadeOut(), mainMenuConfirmationPanelFade.gameObject);
        FadeInPanelAndEnableIt(menuPanelFade.DoFadeIn(), menuPanel.gameObject);
        //fade out exit confirmation
        //fade in menu buttons after exit confirmation is faded out
    }

    public void ExitGame()
    {
        //TODO do saving game data here
        Application.Quit();
        Debug.Log("Application.Quit() call is ignored in editor.");
    }

    public void OpenInventory()
    {
        if(canOpenInventory) 
        {
            
        }
    }

    public void ClosePlayerMenu()
    {

    }

    private void DoFadeIn()
    {
        fadeInCoroutine = dimValueSetter.DoFadeIn();
        bgFadeInCoroutine = backgroundDimValueSetter.DoFadeIn();
        StopCoroutine(fadeOutCoroutine);
        StopCoroutine(bgFadeOutCoroutine);
        StartCoroutine(fadeInCoroutine);
        StartCoroutine(bgFadeInCoroutine);
    }

    private void DoFadeOut()
    {
        fadeOutCoroutine = dimValueSetter.DoFadeOut();
        bgFadeOutCoroutine = backgroundDimValueSetter.DoFadeOut();
        StopCoroutine(fadeInCoroutine);
        StopCoroutine(bgFadeInCoroutine);
        StartCoroutine(fadeOutCoroutine);
        StartCoroutine(bgFadeOutCoroutine);
    }

    private void FadeOutPanelAndDisableIt(IEnumerator coroutine, GameObject objectToDisable)
    {
        StartCoroutine(coroutine);
        objectToDisable.SetActive(false);

    }
    private void FadeInPanelAndEnableIt(IEnumerator coroutine, GameObject objectToEnable)
    {
        objectToEnable.SetActive(true);
        StartCoroutine(coroutine);
    }
}
