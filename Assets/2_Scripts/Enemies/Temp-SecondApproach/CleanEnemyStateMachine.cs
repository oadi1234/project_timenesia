namespace _2_Scripts.Enemies.Temp_SecondApproach
{
    public class CleanEnemyStateMachine : StateMachine
    {
        public readonly ChargeState RightChargeState;
        public readonly ChargeState LeftChargeState;
        public readonly HaltState HaltState;

        public CleanEnemyStateMachine(DynamicEnemyBase dynamicEnemy)
        {
            HaltState = new HaltState(this, dynamicEnemy);
            RightChargeState = new ChargeState(dynamicEnemy, 30f, this);
            LeftChargeState = new ChargeState(dynamicEnemy, -30f, this);
            
            ChangeState(HaltState);
        }
    }
}