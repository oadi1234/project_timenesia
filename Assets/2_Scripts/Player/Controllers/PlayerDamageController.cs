using _2_Scripts.Player.model;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class PlayerDamageController : MonoBehaviour
    {
        public static PlayerDamageController Instance { get; private set; }
        private PlayerMovementController playerMovementController;
        private WeaponStat currentWeapon;


        private void Awake()
        {
            playerMovementController = GetComponent<PlayerMovementController>();
            if (!Instance)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            //TODO load current equipped weapon data from save.
            // Then assign correct weapon to currentWeapon parameter.
            currentWeapon = gameObject.AddComponent<WeaponStat>();
        }

        public WeaponType GetWeaponType()
        {
            return currentWeapon.type;
        }

        public float GetDamage()
        {
            return currentWeapon.damage * (playerMovementController.IsGrounded() ? currentWeapon.groundDamageMult : 1);
        }

        public float GetKnockbackForce()
        {
            return currentWeapon.knockbackStrength * (playerMovementController.IsGrounded() ? currentWeapon.groundDamageMult : 1);
        }

        public float GetSpellMultiplier()
        {
            return currentWeapon.spellDamageMult;
        }
    }
}