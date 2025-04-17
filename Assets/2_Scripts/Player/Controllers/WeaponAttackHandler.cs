using System.Collections;
using _2_Scripts.Global;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.ScriptableObjects;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace _2_Scripts.Player.Controllers
{
    public class WeaponAttackHandler : MonoBehaviour
    {
        [SerializeField] private PolygonCollider2D polyCollider2D;
        public PlayerAttackHandler attackHandler;
        private Vector2 knockbackDirection = Vector2.zero;

        private IEnumerator Start()
        {
            //animation starts here and we want it to run for a few moments before enabling the hitbox
            yield return null; //should be "WaitForSeconds(singleFrame), but for some reason the timing was off.
            polyCollider2D.enabled = true;
            yield return new WaitForSeconds(AC.SingleFrame);
            polyCollider2D.enabled = false;
            Destroy(gameObject);
        }

        public void SetAttackDirection(Vector2 direction)
        {
            knockbackDirection = -direction;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer is (int)Layers.Enemy or (int)Layers.Hazard)
            {
                attackHandler.HandleAttackKnockback(PlayerConstants.Instance.attackObstacleKnockback, knockbackDirection);
            }

            else if (!(knockbackDirection==Vector2.up) == !(knockbackDirection == Vector2.down) && other.gameObject.layer == (int)Layers.Wall)
            {
                attackHandler.HandleAttackKnockback(PlayerConstants.Instance.attackWallKnockback, knockbackDirection);
            }
        }
    }
}