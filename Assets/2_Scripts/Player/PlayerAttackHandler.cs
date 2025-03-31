using _2_Scripts.Player.model;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2_Scripts.Player
{
    public class PlayerAttackHandler : SpriteFacingDirection
    {
        [SerializeField] private GameObject staffAttackZone;

        // TODO logic for attacking.
        private WeaponType currentWeaponType;

        private GameObject staffAttackZoneInstance;

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
        }

        public void SpawnAttackStaffUp()
        {
            staffAttackZoneInstance = Instantiate(staffAttackZone, transform);
            staffAttackZoneInstance.transform.position =
                new Vector3(transform.position.x, transform.position.y + 0.3f, 0f);
            staffAttackZoneInstance.transform.rotation = Quaternion.Euler(0, 0, -90);
        }

        public void SpawnAttackStaffDown()
        {
            staffAttackZoneInstance = Instantiate(staffAttackZone, transform);
            staffAttackZoneInstance.transform.position = new Vector3(transform.position.x - 0.5f * Direction,
                transform.position.y + 0.8f, 0f);
            staffAttackZoneInstance.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        public void SpawnAttackStaffDown2()
        {
            staffAttackZoneInstance = Instantiate(staffAttackZone, transform);
            staffAttackZoneInstance.transform.position = new Vector3(transform.position.x,
                transform.position.y + 0.8f, 0f);
            staffAttackZoneInstance.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }
}