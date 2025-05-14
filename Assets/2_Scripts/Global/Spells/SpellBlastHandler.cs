using System.Collections;
using _2_Scripts.Player.Animation.model;
using UnityEngine;

namespace _2_Scripts.Global.Spells
{
    public class SpellBlastHandler : MonoBehaviour
    {
        [SerializeField] private Collider2D spellCollider;

        [SerializeField] private float destroyAfter = 5/12f;
        // Start is called before the first frame update
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(AC.SingleFrame);
            spellCollider.enabled = false;
            yield return new WaitForSeconds(destroyAfter);
            Destroy(gameObject);
        }
    }
}
