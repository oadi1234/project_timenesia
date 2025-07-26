using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Global.Spells
{
    public abstract class AbstractDamagingSpell : MonoBehaviour
    {
        public float spellDamage;
        public float spellKnockback;

        [SerializeField] private bool isMultihit = false;
        
        private HashSet<int> taggedEnemyIds = new();
        
        public bool IsMultihit => isMultihit;

        public void TagEnemy(GameObject enemy)
        {
            taggedEnemyIds.Add(enemy.GetInstanceID());
        }

        public bool WasTagged(GameObject enemy)
        {
            return taggedEnemyIds.Contains(enemy.GetInstanceID());
        }
    }
}