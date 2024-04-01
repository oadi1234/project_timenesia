namespace _2_Scripts.Global.FSM
{
    public interface IState
    {
        void OnLogic();
        void OnEnter();
        void OnExit();
    }
}