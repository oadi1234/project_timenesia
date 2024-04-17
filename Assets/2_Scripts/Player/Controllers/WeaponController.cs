using System;
using System.Collections;
using _2_Scripts.ExtensionMethods;
using _2_Scripts.Global;
using _2_Scripts.Model;
using Unity.Mathematics;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private WeaponBase staff;
        [SerializeField] private WeaponBase wand;

        private WeaponBase _currentWeapon;
        private float _currentCooldown;
        public bool IsAttacking => _currentWeapon != null && _currentWeapon.gameObject.activeSelf;
        public int CurrentDamage => _currentWeapon != null ? _currentWeapon.Damage : 0;
        private Transform CurrentWeaponTransform => _currentWeapon.transform;

        public void QuickChangeWeapon()
        {
            if (_currentWeapon is null)
                _currentWeapon = staff;

            else
                switch (_currentWeapon.WeaponType)
                {
                    case Weapon.Staff:
                        _currentWeapon = wand;
                        break;
                    case Weapon.Sword:
                        break;
                    case Weapon.Rod:
                        break;
                    case Weapon.Orb:
                        break;
                    case Weapon.Wand:
                        _currentWeapon = staff;
                        break;
                    default:
                        // throw new ArgumentOutOfRangeException(nameof(weapon), weapon, null);
                        _currentWeapon = null;
                        break;
                }
        }

        public void ChangeWeapon(Weapon weapon)
        {
            switch (weapon)
            {
                case Weapon.Staff:
                    _currentWeapon = staff;
                    break;
                case Weapon.Sword:
                    break;
                case Weapon.Rod:
                    break;
                case Weapon.Orb:
                    break;
                case Weapon.Wand:
                    _currentWeapon = wand;
                    break;
                default:
                    // throw new ArgumentOutOfRangeException(nameof(weapon), weapon, null);
                    _currentWeapon = null;
                    break;
            }
        }
        
        public void Attack(Direction direction)
        {
            if (_currentCooldown > 0 || _currentWeapon is null) return;
            
            switch (direction)
            {
                case Direction.LEFT:
                    CurrentWeaponTransform.localScale = CurrentWeaponTransform.localScale.MirrorHorizontally();
                    break;
                case Direction.UP:
                    CurrentWeaponTransform.Rotate(0,0,90);
                    CurrentWeaponTransform.localPosition = _currentWeapon.UpPosition;
                    break;
                case Direction.RIGHT:
                    break;
            }
            
            _currentWeapon.gameObject.SetActive(true);
            _currentCooldown = _currentWeapon.Cooldown;
            StartCoroutine(MakeAttack(direction == Direction.LEFT, direction == Direction.UP));
        }

        private IEnumerator MakeAttack(bool mirrorAtTheEnd = false, bool resetRotationAndPosition = false)
        {
            yield return new WaitForSeconds(0.1f);
            _currentWeapon.gameObject.SetActive(false);
            if (mirrorAtTheEnd)
            {
                CurrentWeaponTransform.localScale = CurrentWeaponTransform.localScale.MirrorHorizontally();
            }

            if (resetRotationAndPosition)
            {
                CurrentWeaponTransform.Rotate(0,0,-90);
                CurrentWeaponTransform.localPosition = Vector3.zero;
            }
        }

        private void FixedUpdate()
        {
            if (_currentCooldown>0)
            {
                _currentCooldown -= Time.fixedDeltaTime;
            }
        }
    }
}