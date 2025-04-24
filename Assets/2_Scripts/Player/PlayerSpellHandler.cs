using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using _2_Scripts.Player.ScriptableObjects;
using _2_Scripts.Player.Statistics;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerSpellHandler : MonoBehaviour
    {
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private PlayerInputManager playerInputManager;
        [SerializeField] private PlayerEffort playerEffort;

        public void Concentrate()
        {
            //TODO set playerInputManager to concentration mode - no movement, accept keypresses as mana directions
            //TODO set player effort to concentration mode - increased effort regen
            //TODO handle on concentration exit logic - either nothing, dud spell (wild magic?) or full spell
        }

        public void CastSpell()
        {
            //TODO read current spell combination from PlayerEffort class
            // then invoke spellbook with given id i guess
        }
    
        public void StartAngleMode()
        {
            playerInputManager.SetAdjustAngleMode(true);
        }

        public void DoLungeOnHeavySpellcast()
        {
            var angle = playerInputManager.GetAngle();
            var direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

            playerMovementController.AttackLunge(PlayerConstants.Instance.staffLungeMagnitude * direction.x,
                AC.StaffHeavySpellcastLungeTimer, direction.y * PlayerConstants.Instance.staffLungeMagnitude);
            playerInputManager.SetAngleModeAdjustStrength(1f);
        }

        public void EndAngleMode()
        {
            playerInputManager.SetAdjustAngleMode(false);
            playerInputManager.SetAngle(0f);
            playerInputManager.SetAngleModeAdjustStrength(3f);
        }
    }
}
