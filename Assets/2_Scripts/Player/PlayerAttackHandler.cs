using _2_Scripts.Player.Controllers;
using _2_Scripts.Player.model;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerAttackHandler : SpriteFacingDirection
    {
        [SerializeField] private GameObject staffAttackZone;
        [SerializeField] private PlayerMovementController playerMovementController;

        // TODO logic for attacking.
        private WeaponType currentWeaponType;

        private GameObject staffAttackZoneInstance;

        protected override void Awake()
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
                .SetKnockbackDirection(Direction > 0 ? Vector2.right : Vector2.left);
        }

        public void SpawnAttackStaffUp()
        {
            staffAttackZoneInstance = Instantiate(staffAttackZone, transform);
            staffAttackZoneInstance.transform.position =
                new Vector3(transform.position.x, transform.position.y + 0.3f, 0f);
            staffAttackZoneInstance.transform.rotation = Quaternion.Euler(0, 0, -90);
            staffAttackZoneInstance.GetComponent<WeaponAttackHandler>().SetKnockbackDirection(Vector2.down);
        }

        public void SpawnAttackStaffDown()
        {
            staffAttackZoneInstance = Instantiate(staffAttackZone, transform);
            staffAttackZoneInstance.transform.position = new Vector3(transform.position.x - 0.5f * Direction,
                transform.position.y + 0.8f, 0f);
            staffAttackZoneInstance.transform.rotation = Quaternion.Euler(0, 0, 90);
            staffAttackZoneInstance.GetComponent<WeaponAttackHandler>().SetKnockbackDirection(Vector2.up);
        }

        public void SpawnAttackStaffDown2()
        {
            staffAttackZoneInstance = Instantiate(staffAttackZone, transform);
            staffAttackZoneInstance.transform.position = new Vector3(transform.position.x,
                transform.position.y + 0.8f, 0f);
            staffAttackZoneInstance.transform.rotation = Quaternion.Euler(0, 0, 90);
            staffAttackZoneInstance.GetComponent<WeaponAttackHandler>().SetKnockbackDirection(Vector2.up);
        }

        public void HandleAttackKnockback(float strength, Vector2 direction)
        {
            playerMovementController.AttackKnockback(strength, direction);
        }
    }
}