using _2_Scripts.Player.model;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class PlayerDamageController : MonoBehaviour
    {
        public static PlayerDamageController Instance { get; private set; }
        private PlayerMovementController playerMovementController;
        private WeaponStat currentWeapon;
        public float currentDamage { get; private set; }
        public float currentKnockbackForce { get; private set; }


        private void Awake()
        {
            playerMovementController = GetComponent<PlayerMovementController>();
            if (Instance == null)
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
        
        private void FixedUpdate()
        {
            currentDamage = currentWeapon.damage *
                            (playerMovementController.IsGrounded() ? currentWeapon.groundDamageMult : 1);
            currentKnockbackForce = currentWeapon.knockbackStrength * (playerMovementController.IsGrounded() ? currentWeapon.groundDamageMult : 1);
        }

        public WeaponType GetWeaponType()
        {
            return currentWeapon.type;
        }
    }
}