namespace _2_Scripts.Enemies.States
{
    public interface IState
    {
        void OnEnter();
        void OnExit();
        void OnLogic();
    }
}