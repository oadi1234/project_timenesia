using UnityEngine;

namespace _2_Scripts.Global.Health.EnemyAttack
{
    public class DamageParameters
    {
        public int DamageDealt = 1;
        public float IFramesGiven = 1.5f;
        public bool ForcesRespawnAtCheckpoint = false;
        public float KnockbackStrength = 10f;
        public bool BlocksMovement = false;
        public bool HeavyAttack = false;
        public Vector2 DamageSourcePosition;
    }
}
