using System.Collections;
using _2___Scripts.Global;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2_Scripts.Enemies.Temp_Spitter
{
    public class SpitterAgent : StaticEnemyBase, IEnemyWithInstantiate
    {
        private SpitterStateMachine _stateMachine;
        private CircleCollider2D _sight;
        public GameObject theSpit;

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new SpitterStateMachine(this, theSpit);
            _sight = GetComponent<CircleCollider2D>();
            _stateMachine.ChangeState(_stateMachine.HaltState);
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == (int)LayerNames.Player)
            {
                SetSightEnabled(false);
                _stateMachine.ChangeState(_stateMachine.SpitProjectilesState);
            }
        }        
        // private void OnTriggerExit2D(Collider2D collision)
        // {
        //     if (collision.gameObject.layer == (int)LayerNames.Player)
        //     {
        //         _stateMachine.ChangeState(_stateMachine.SpitProjectilesState);
        //     }
        // }

        public override void SetSightEnabled(bool sightEnabled)
        {
            _sight.enabled = sightEnabled;
        }

        public void InstantiateObject(GameObject objectToInstantiate, bool inWorldSpace = false)
        {
            Debug.Log("Instant - parent");
            var projectile = Instantiate(objectToInstantiate, transform, false);
            projectile.transform.parent = null;
            projectile.GetComponent<Rigidbody2D>().AddForce(5*(Random.insideUnitCircle + new Vector2(0,1.2f)), ForceMode2D.Impulse);
        }
    }
}