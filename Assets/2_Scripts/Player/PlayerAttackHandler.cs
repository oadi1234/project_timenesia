using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using _2_Scripts.Player.model;
using _2_Scripts.Player.ScriptableObjects;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerAttackHandler : SpriteFacingDirection
    {
        [SerializeField] private GameObject staffAttackZone;
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private PlayerInputManager playerInputManager;

        // TODO logic for attacking.
        private WeaponType currentWeaponType;

        private GameObject staffAttackZoneInstance;

        public override void Awake()
        {
            base.Awake();
            staffAttackZone.GetComponent<WeaponAttackHandler>().attackHandler = this;
        }

        private void Update()
        {
            CalculateDirection();
        }

        public void SpawnAttackStaff()
        {
            staffAttackZoneInstance = Instantiate(staffAttackZone, transform);
            staffAttackZoneInstance.transform.localScale = new Vector3(Direction, 1, 1);
            staffAttackZoneInstance.transform.position = new Vector3(transform.position.x,
                transform.position.y + 0.67f, 0);
            staffAttackZoneInstance.GetComponent<WeaponAttackHandler>()
                .SetAttackDirection(Direction > 0 ? Vector2.left : Vector2.right);
        }

        public void SpawnAttackStaffUp()
        {
            staffAttackZoneInstance = Instantiate(staffAttackZone, transform);
            staffAttackZoneInstance.transform.position =
                new Vector3(transform.position.x, transform.position.y + 0.3f, 0f);
            staffAttackZoneInstance.transform.rotation = Quaternion.Euler(0, 0, -90);
            staffAttackZoneInstance.GetComponent<WeaponAttackHandler>().SetAttackDirection(Vector2.up);
        }

        public void SpawnAttackStaffDown()
        {
            staffAttackZoneInstance = Instantiate(staffAttackZone, transform);
            staffAttackZoneInstance.transform.position = new Vector3(transform.position.x - 0.5f * Direction,
                transform.position.y + 0.8f, 0f);
            staffAttackZoneInstance.transform.rotation = Quaternion.Euler(0, 0, 90);
            staffAttackZoneInstance.GetComponent<WeaponAttackHandler>().SetAttackDirection(Vector2.down);
        }

        public void SpawnAttackStaffDown2()
        {
            staffAttackZoneInstance = Instantiate(staffAttackZone, transform);
            staffAttackZoneInstance.transform.position = new Vector3(transform.position.x,
                transform.position.y + 0.8f, 0f);
            staffAttackZoneInstance.transform.rotation = Quaternion.Euler(0, 0, 90);
            staffAttackZoneInstance.GetComponent<WeaponAttackHandler>().SetAttackDirection(Vector2.down);
        }

        public void HandleAttackKnockback(float strength, Vector2 direction)
        {
            playerMovementController.AttackKnockback(strength, direction);
        }

        //TODO move below to spell handler.
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
            playerInputManager.SetAngleModeAdjustStrength(6f);
        }

        public void EndAngleMode()
        {
            playerInputManager.SetAdjustAngleMode(false);
            playerInputManager.SetAngle(0f);
            playerInputManager.SetAngleModeAdjustStrength(3f);
        }
    }
}