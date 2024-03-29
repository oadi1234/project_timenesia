using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * TODO set as the script for the actual main menu.
 */
public class MainMenu : MonoBehaviour
{
    public GameObject Main_Menu;
    public GameObject Credits_Menu;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuButton();
    }

    public void PlayNowButton()
    {
        // Play Now Button has been pressed, here you can initialize your game (For example Load a Scene called GameLevel etc.)
        // UnityEngine.SceneManagement.SceneManager.LoadScene("GameLevel");
    }

    public void CreditsButton()
    {
        // Show Credits Menu
        Main_Menu.SetActive(false);
        Credits_Menu.SetActive(true);
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        Main_Menu.SetActive(false);
    }

    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
    }

    public void Save()
    {
        SoundManager.Instance.PlayOnce(GlobalAssets.Instance.SaveAudioClip);
    }
}
