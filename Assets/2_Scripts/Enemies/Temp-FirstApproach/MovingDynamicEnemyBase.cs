using System;

namespace _2_Scripts.Enemies.Temp_FirstApproach
{
    public abstract class MovingDynamicEnemyBase : DynamicEnemyBase
    {
        protected StateMachine MovingStateMachine;       
        protected void At(IState from, IState to, Func<bool> condition, StateMachine stateMachine) => stateMachine.AddTransition(from, to, condition);

        protected override void Awake()
        {
            base.Awake();
            MovingStateMachine = new StateMachine();
        }

        protected void FixedUpdate()
        {
            MovingStateMachine.OnLogic();
        }

    }
}