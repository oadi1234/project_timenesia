using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{ 
    public PlayerController playerController;
    
    public void Test(int spellIndex)
    {
        playerController.Test(spellIndex);
    }
}
