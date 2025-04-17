namespace _2_Scripts.Player.Animation.model
{
    public interface IStateHandler
    {
        public int GetCurrentState();

        public int GetCurrentHurtState();

        public bool LockXFlip();

        bool ShouldRestartAnim();
    }
}