using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
