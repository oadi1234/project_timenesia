using _2_Scripts.Enemies.FSM;
using _2_Scripts.Global;
using UnityEngine;

namespace _2_Scripts.Enemies.Agents
{
    public class SpitterAgent : StaticEnemyBase, IEnemyWithInstantiate
    {
        private SpitterStateMachine _stateMachine;
        private CircleCollider2D _sight;
        public GameObject theSpit;
        private float angleRange = 90;
        private float angleOffset = 45;
        private float minSpeed = 5;
        private float maxSpeed = 10;

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new SpitterStateMachine(this, theSpit);
            _sight = GetComponent<CircleCollider2D>();
            _stateMachine.ChangeState(_stateMachine.HaltState);
        }
        
        public override void OnSight(Collider2D other)
        {
            if (other.gameObject.layer == (int)Layers.Player)
            {
                SetSightEnabled(false);
                _stateMachine.ChangeState(_stateMachine.SpitProjectilesState);
            }
        }        

        public override void SetSightEnabled(bool sightEnabled)
        {
            _sight.enabled = sightEnabled;
        }

        public void InstantiateObject(GameObject objectToInstantiate, bool inWorldSpace = false)
        {
            var projectile = Instantiate(objectToInstantiate, transform, false);
            projectile.transform.parent = null;
            var speed = Random.Range(minSpeed, maxSpeed);
            projectile.GetComponent<Rigidbody2D>().AddForce(speed * RandomVector2(  Mathf.Deg2Rad * angleRange, Mathf.Deg2Rad * angleOffset), ForceMode2D.Impulse);
            // projectile.GetComponent<Rigidbody2D>().AddForce(5*(Random.insideUnitCircle + new Vector2(0,1.2f)), ForceMode2D.Impulse);
        }
        
        public static Vector2 DegreeToVector2(float degree)
        {  
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }
        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
        
        public Vector2 RandomVector2(float angle, float angleMin){
            float random = Random.value * angle + angleMin;
            return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
        }
        
    }
}