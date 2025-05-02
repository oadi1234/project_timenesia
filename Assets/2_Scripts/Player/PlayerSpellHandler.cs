using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using _2_Scripts.Player.ScriptableObjects;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerSpellHandler : MonoBehaviour
    {
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private PlayerInputManager playerInputManager;

        public void Concentrate()
        {
            playerInputManager.SetConcentration(true);
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