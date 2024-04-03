using UnityEngine;

namespace _2_Scripts.Enemies.Controllers
{
    public class SightController : MonoBehaviour
    {
        private StaticEnemyBase _enemyBase;
        // Start is called before the first frame update
        void Start()
        {
            _enemyBase = GetComponentInParent<StaticEnemyBase>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            _enemyBase.OnSight(collision);
        }

    }
}
