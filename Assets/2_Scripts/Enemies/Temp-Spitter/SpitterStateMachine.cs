using _2_Scripts.Enemies.Temp_SecondApproach;
using UnityEngine;

namespace _2_Scripts.Enemies.Temp_Spitter
{
    public class SpitterStateMachine : StateMachine
    {
        public readonly SpitProjectilesState SpitProjectilesState;
        public readonly HaltState HaltState;
        
        public SpitterStateMachine(StaticEnemyBase enemy, GameObject theSpit)
        {
            HaltState = new HaltState(this, enemy);
            SpitProjectilesState = new SpitProjectilesState(enemy, theSpit, this);
            
            ChangeState(HaltState);
        }
        
    }
}