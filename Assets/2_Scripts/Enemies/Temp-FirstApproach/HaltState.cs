using UnityEngine;

namespace _2_Scripts.Enemies.Temp_FirstApproach
{
    public class HaltState : StateBase
    {
        private DynamicEnemyBase _dynamicEnemy;

        public HaltState(DynamicEnemyBase dynamicEnemy)
        {
            _dynamicEnemy = dynamicEnemy;
        }

        public override void OnEnter()
        {
            _dynamicEnemy.SpriteRenderer.color = Color.white;
        }
    }
}