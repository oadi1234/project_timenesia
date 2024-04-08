using System;
using UnityEngine;

namespace _2_Scripts.UI.Elements.MainMenu
{
    public class StartButton : MonoBehaviour
    {
        public string directoryName {  get; set; }
        public static event Action<string> NewGame;

        public void Click()
        {
            if (NewGame != null)
            {
                NewGame(directoryName);
            }
        }

        public static void StartNewGame() //for starting the first game.
        {
            if (NewGame != null)
            {
                NewGame("save_0");
            }
        }
    }
}
