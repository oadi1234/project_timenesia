namespace _2_Scripts.Global.Animation.Model
{
    public interface IStateHandler
    {
        public int GetCurrentState();

        public int GetCurrentHurtState();
        
        public bool LockXFlip();

        public bool ShouldRestartAnim();
    }
}