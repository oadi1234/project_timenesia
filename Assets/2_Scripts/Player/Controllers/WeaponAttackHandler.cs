using System.Collections;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class WeaponAttackHandler : MonoBehaviour
    {
        private IEnumerator Start()
        {
            //animation starts here and we want it to run for a few moments before enabling the hitbox
            yield return new WaitForSeconds(1f/12f);
            //enable hitbox
            yield return new WaitForSeconds(1f/12f);
            Destroy(gameObject);
        }
    }
}
