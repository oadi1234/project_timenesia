using System;
using _2_Scripts.Global.FSM;

namespace _2_Scripts.Enemies
{
    public abstract class MovingEnemyBase : EnemyBase
    {
        protected StateMachine MovingStateMachine;

        protected override void Awake()
        {
            base.Awake();
            MovingStateMachine = new StateMachine();
        }

        protected void Update()
        {
            MovingStateMachine.OnLogic();
        }

        protected void At(IState from, IState to, Func<bool> condition, StateMachine stateMachine) => stateMachine.AddTransition(from, to, condition);
    }
}