namespace _2_Scripts.Player.Animation.model
{
    public class AnimationData
    {
        public int animationStateHash {get; private set;}
        public float animationDuration {get; private set;}
        public float animationLockDuration {get; private set;}
        public float animationHangDuration { get; private set; }
        
        public AnimationData chainsInto { get; private set; }

        public AnimationData(int animationStateHash, float animationDuration, float animationLockDuration)
        {
            this.animationStateHash = animationStateHash;
            this.animationLockDuration = animationLockDuration;
            this.animationDuration = animationDuration;
            this.animationHangDuration = animationLockDuration;
        }

        public AnimationData(int animationStateHash, float animationDuration, float animationLockDuration,
            float animationHangDuration)
        {
            this.animationStateHash = animationStateHash;
            this.animationLockDuration = animationLockDuration;
            this.animationDuration = animationDuration;
            this.animationHangDuration = animationHangDuration;
        }

        public void SetChainsInto(ref AnimationData chainsIntoRef)
        {
            chainsInto = chainsIntoRef;
        }
    }
}