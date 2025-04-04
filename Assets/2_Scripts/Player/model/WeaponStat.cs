using UnityEngine;

namespace _2_Scripts.Player.model
{
    //TODO I presume this would be best stored as some sort of dictionary with weaponid:weapon stats somewhere.
    public class WeaponStat : MonoBehaviour
    {
        public WeaponType type = WeaponType.Staff;
        public float damage = 10;
        public float groundDamageMult = 1.5f; //damage when standing on ground. Since it "stops" character a bit it should be stronger.
        public float spellDamageMult = 1f;
        public float knockbackStrength = 0.5f;
    }
}