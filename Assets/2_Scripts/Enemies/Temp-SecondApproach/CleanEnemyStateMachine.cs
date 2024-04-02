using _2_Scripts.Enemies.Temp_SecondApproach;
using UnityEngine;

namespace _2_Scripts.Enemies.Temp_SecondApproach
{
    public class CleanEnemyStateMachine : IStateMachine
    {
        public ChargeState RightChargeState;
        public ChargeState LeftChargeState;
        public HaltState HaltState;

        private IState _currentState;

        public CleanEnemyStateMachine(EnemyBase enemy)
        {
            HaltState = new HaltState(this, enemy);
            RightChargeState = new ChargeState(enemy, 3f, this);
            LeftChargeState = new ChargeState(enemy, -3f, this);

            _currentState = HaltState;
        }
        public void OnUpdate()
        {
            _currentState?.OnUpdate();
        }
        
        public void ChangeState(IState newState)
        {
            _currentState?.OnExit();
            _currentState = newState;
            _currentState.OnEnter();
        }
    }
}