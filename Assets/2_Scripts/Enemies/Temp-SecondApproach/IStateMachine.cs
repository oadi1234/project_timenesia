namespace _2_Scripts.Enemies.Temp_SecondApproach
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