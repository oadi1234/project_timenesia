using _2_Scripts.Player.model;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class PlayerDamageController : MonoBehaviour
    {
        // public static PlayerDamageController Instance { get; private set; }
        private PlayerMovementController playerMovementController;
        private WeaponStat currentWeapon;
        public static float currentDamage { get; private set; }
        public static float currentKnockbackForce { get; private set; }


        private void Awake()
        {
            playerMovementController = GetComponent<PlayerMovementController>();
            // if (Instance == null)
            // {
            //     Instance = this;
            // }
            // else if (Instance != this)
            // {
            //     Destroy(gameObject);
            // }
            //TODO load current equipped weapon data from save.
            // Then assign correct weapon to currentWeapon parameter.
            currentWeapon = gameObject.AddComponent<WeaponStat>();
        }
        
        private void FixedUpdate()
        {
            //TODO this parameter should also be used when casting spells for simplicity - the logic will need to be adjusted
            currentDamage = currentWeapon.damage *
                            (playerMovementController.IsGrounded() ? currentWeapon.groundDamageMult : 1);
            currentKnockbackForce = currentWeapon.knockbackStrength * (playerMovementController.IsGrounded() ? currentWeapon.groundDamageMult : 1);
        }

        public WeaponType GetWeaponType()
        {
            return currentWeapon.type;
        }

        // TODO commented out because this functionality might be more complicated in the future.
        // public void ChangeWeapon(WeaponType weaponType)
        // {
        //     switch (weaponType)
        //     {
        //         case WeaponType.Staff:
        //             _currentWeapon = staff;
        //             break;
        //         case WeaponType.Sword:
        //             break;
        //         case WeaponType.Rod:
        //             break;
        //         case WeaponType.Orb:
        //             break;
        //         case WeaponType.Daggerwand:
        //             _currentWeapon = wand;
        //             break;
        //         default:
        //             // throw new ArgumentOutOfRangeException(nameof(weapon), weapon, null);
        //             _currentWeapon = null;
        //             break;
        //     }
        // }
    }
}