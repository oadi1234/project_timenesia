using _2_Scripts.Enemies.States;
using UnityEngine;

namespace _2_Scripts.Enemies.FSM
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