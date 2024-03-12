using UnityEngine;

namespace _2___Scripts.Player
{
    public class Hurt : MonoBehaviour
    {
        public int damageDealt = 2;
        public float iFramesGiven = 2f;
        public bool forcesRespawnAtCheckpoint = false;
        public float knockbackStrength = 10f;
        public bool blocksMovement = false;
    }
}
