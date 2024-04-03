using UnityEngine;

namespace _2_Scripts.Enemies
{
    public abstract class DynamicEnemyBase : StaticEnemyBase
    {
        public Rigidbody2D RigidBody { get; private set; }
        protected new virtual void Awake()
        {
            base.Awake();
            RigidBody = GetComponent<Rigidbody2D>();
        }
    }
}