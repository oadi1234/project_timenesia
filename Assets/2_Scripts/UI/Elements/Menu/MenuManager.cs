using System;
using System.Collections;
using _2_Scripts.Global.SaveSystem;
using _2_Scripts.Scenes;
using _2_Scripts.UI.Elements.Enum;
using UnityEngine;

namespace _2_Scripts.UI.Elements.Menu
{
    public class MenuManager : MonoBehaviour
    {
        public FadeController dimValueSetter;
        public FadeController backgroundDimValueSetter;
        public UIPanel menuPanel;
        public UIPanel mainMenuConfirmationPanel;
        public UIPanel exitConfirmationPanel;
        public PlayerUIPanel playerMenuPanel;

        private bool canOpenPlayerMenu = true;
        private IEnumerator fadeInCoroutine;
        private IEnumerator fadeOutCoroutine;
        private IEnumerator bgFadeInCoroutine;
        private IEnumerator bgFadeOutCoroutine;
        private UIWindowType currentlyOpenMenu = UIWindowType.None;
        private Color32 foregroundFadeColour = new Color32(55, 43, 21, 191); // TODO placeholders for now, but could be cool to make these change depending on the in game location.
        private Color32 backgroundFadeColour = new Color32(107, 81, 42, 191);
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
                    OpenInGameMenu();
                    break;
                case UIWindowType.ExitPanel:
                    CloseExitGameConfirmationMenu();
                    break;
                case UIWindowType.InGameMenu:
                    CloseInGameMenu();
                    break;
                case UIWindowType.MainMenuPanel:
                    CloseMainMenuConfirmationMenu();
                    break;
                case UIWindowType.PlayerMenu:
                    ClosePlayerMenu();
                    break;
                default:
                    break;
            }
        }

        public void CloseInGameMenu()
        {
            currentlyOpenMenu = UIWindowType.None;
            menuPanel.ToggleActive();
            DoFadeOut();
            canOpenPlayerMenu = true;
        }

        public void OpenInGameMenu()
        {
            currentlyOpenMenu = UIWindowType.InGameMenu;
            menuPanel.ToggleActive();
            DoFadeIn();
            canOpenPlayerMenu = false;
        }

        public void OpenOptionsSubmenu()
        {
            Debug.Log("Options Menu"); //TODO put options menu here when its done for the main menu.
        }

        public void CloseOptionsSubmenu()
        {

        }

        public void GoToMainMenu()
        {
            SaveManager.Instance.Save();
            SceneLoader.Instance.LoadMainMenu();
            SceneDataHolder.Instance.ClearLoadStrategy();
        }

        public void OpenExitGameConfirmationMenu()
        {
            currentlyOpenMenu = UIWindowType.ExitPanel;
            exitConfirmationPanel.ToggleActive();
            exitConfirmationPanel.SetButtonToBeSelectedOnActive(0);
            menuPanel.ToggleActive();
        }

        public void CloseExitGameConfirmationMenu()
        {
            currentlyOpenMenu = UIWindowType.InGameMenu;
            exitConfirmationPanel.ToggleActive();
            menuPanel.SetButtonToBeSelectedOnActive(0);
            menuPanel.ToggleActive();
        }

        public void OpenMainMenuConfirmationMenu()
        {
            currentlyOpenMenu = UIWindowType.MainMenuPanel;
            mainMenuConfirmationPanel.ToggleActive();
            mainMenuConfirmationPanel.SetButtonToBeSelectedOnActive(0);
            menuPanel.ToggleActive();
        }

        public void CloseMainMenuConfirmationMenu()
        {
            currentlyOpenMenu = UIWindowType.InGameMenu;
            mainMenuConfirmationPanel.ToggleActive();
            menuPanel.SetButtonToBeSelectedOnActive(0);
            menuPanel.ToggleActive();
        }

        public void ExitGame()
        {
            //TODO do saving game data here
            Application.Quit();
            Debug.Log("Application.Quit() call is ignored in editor.");
        }

        public void TogglePlayerMenu()
        {
            if(canOpenPlayerMenu) 
            {
                OpenPlayerMenu();
            }
            else if (currentlyOpenMenu==UIWindowType.PlayerMenu)
            {
                ClosePlayerMenu();
            }
        }

        private void OpenPlayerMenu()
        {
            currentlyOpenMenu = UIWindowType.PlayerMenu;
            playerMenuPanel.Open();
            playerMenuPanel.OpenPlayerMenuUI();
            canOpenPlayerMenu = false;
        }

        private void ClosePlayerMenu()
        {
            currentlyOpenMenu = UIWindowType.None;
            playerMenuPanel.ClosePlayerMenuUI();
            canOpenPlayerMenu = true;
        }

        private void DoFadeIn()
        {
            dimValueSetter.fadeColour.color = foregroundFadeColour;
            backgroundDimValueSetter.fadeColour.color = backgroundFadeColour;
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
    }
}
