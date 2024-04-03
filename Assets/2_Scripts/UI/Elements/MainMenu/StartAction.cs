using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAction : MonoBehaviour
{

    public static event Action NewGame;

    public void Click()
    {
        if (NewGame != null)
        {
            NewGame();
        }
    }

    public static void StartNewGame() //for starting the first game.
    {
        if (NewGame != null)
        {
            NewGame();
        }
    }
}
