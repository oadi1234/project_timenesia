using _2_Scripts.Player.Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2_Scripts.Player.Animation
{
    public class PlayerAnimationEvents : MonoBehaviour
    { 
        public PlayerSpellController playerSpellController;
    
        public void Test(int spellIndex)
        {
            playerSpellController.Test(spellIndex);
        }
    }
}
