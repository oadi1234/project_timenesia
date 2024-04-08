using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player.Animation
{
    public class PlayerAnimationEvents : MonoBehaviour
    { 
        public PlayerAttackController playerAttackController;
    
        public void Test(int spellIndex)
        {
            playerAttackController.Test(spellIndex);
        }
    }
}
