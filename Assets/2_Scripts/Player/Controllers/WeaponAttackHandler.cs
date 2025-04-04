using System.Collections;
using _2_Scripts.Player.Animation.model;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class WeaponAttackHandler : MonoBehaviour
    {
        [SerializeField] private PolygonCollider2D polyCollider2D;
        private IEnumerator Start()
        {
            //animation starts here and we want it to run for a few moments before enabling the hitbox
            yield return new WaitForSeconds(AC.SingleFrame);
            polyCollider2D.enabled = true;
            yield return new WaitForSeconds(AC.SingleFrame);
            Destroy(gameObject);
        }
    }
}
