using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{ 
    public PlayerAttackController playerAttackController;
    
    public void Test(int spellIndex)
    {
        playerAttackController.Test(spellIndex);
    }
}
