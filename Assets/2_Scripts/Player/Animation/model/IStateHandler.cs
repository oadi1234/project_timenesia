namespace _2_Scripts.Player.Animation.model
{
    public interface IStateHandler
    {
        public int GetCurrentState();

        public bool LockXFlip();

        bool ShouldRestartAnim();
    }
}