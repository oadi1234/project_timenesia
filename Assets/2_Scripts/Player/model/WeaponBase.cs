using _2_Scripts.Player.model;
using UnityEngine;

namespace _2_Scripts.Model
{
    public class WeaponBase : MonoBehaviour
    {
        public int Damage;
        public Vector3 UpPosition;
        public WeaponType WeaponType;
        public float Cooldown = 1f;
    }
}