using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _2_Scripts.Global.Spells
{
    public class DamagingSpell : MonoBehaviour
    {
        public float spellDamage;
        public float spellKnockback;

        [SerializeField] private float disableCollisionAfter = 0.2f;

        //TODO change this so it has some interval in multihit behaviour instead of doing it every physics cycle.
        [Tooltip("Controls whether spell hits only once per physics cycle or not.")]
        [SerializeField] private bool isMultihit = false;
        
        private HashSet<int> taggedEnemyIds = new();
        
        public bool IsMultihit => isMultihit;

        private IEnumerator Start()
        {
            var spellCollision = GetComponent<Collider2D>();
            if (spellCollision && disableCollisionAfter > 0f)
            {
                yield return new WaitForSeconds(disableCollisionAfter);
                spellCollision.enabled = false;
            }
        }

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