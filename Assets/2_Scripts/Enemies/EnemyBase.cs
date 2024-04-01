using UnityEngine;

namespace _2_Scripts.Enemies
{
    public abstract class EnemyBase : MonoBehaviour //TODO: remove MB and change Awake to ctor
    {
        public Rigidbody2D RigidBody { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }

        protected virtual void Awake()
        {
            RigidBody = GetComponent<Rigidbody2D>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}