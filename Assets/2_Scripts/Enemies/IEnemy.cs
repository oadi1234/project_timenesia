using System;

namespace _2_Scripts.Enemies
{
    internal interface IEnemy
    {
        public event Action<int> OnEnemyKilled;
    }
}
