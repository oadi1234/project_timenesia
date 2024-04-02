namespace _2_Scripts.Global.FSM
{
    public abstract class StateBase : IState
    {
        public virtual void OnLogic()
        {
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}