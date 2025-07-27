namespace _2_Scripts.Player.Animation.model
{
    public class AnimationData
    {
        public int AnimationStateHash {get; private set;}
        public float AnimationDuration {get; private set;}
        public float AnimationLockDuration {get; private set;}
        public float AnimationHangDuration { get; private set; } //for duration of air hangs on certain spells.
        
        public AnimationData ChainsInto { get; private set; }

        public AnimationData(int animationStateHash, float animationDuration, float animationLockDuration)
        {
            this.AnimationStateHash = animationStateHash;
            this.AnimationLockDuration = animationLockDuration;
            this.AnimationDuration = animationDuration;
            AnimationHangDuration = animationLockDuration;
        }

        public void SetDurationsTo(float value)
        {
            AnimationLockDuration = value;
            AnimationDuration = value;
        }

        public void SetChainsInto(ref AnimationData chainsIntoRef)
        {
            ChainsInto = chainsIntoRef;
        }
    }
}