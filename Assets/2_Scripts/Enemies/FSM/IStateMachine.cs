using _2_Scripts.Enemies.States;

namespace _2_Scripts.Enemies.FSM
{
    public abstract class StateMachine
    {
        public IState CurrentState { get; private set; }

        public void ChangeState(IState newState)
        {
            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState.OnEnter();
        }
        public virtual void OnUpdate()
        {
            CurrentState?.OnLogic();
        }
    }
}