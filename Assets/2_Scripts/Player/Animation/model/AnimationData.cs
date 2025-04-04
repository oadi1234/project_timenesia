namespace _2_Scripts.Player.Animation.model
{
    public class AnimationData
    {
        public int animationStateHash {get; private set;}
        public float animationDuration {get; private set;}
        public float animationLockDuration {get; private set;}
        
        public AnimationData chainsInto { get; private set; }

        public AnimationData(int animationStateHash, float animationDuration, float animationLockDuration)
        {
            this.animationStateHash = animationStateHash;
            this.animationLockDuration = animationLockDuration;
            this.animationDuration = animationDuration;
        }

        public void SetChainsInto(ref AnimationData chainsIntoRef)
        {
            chainsInto = chainsIntoRef;
        }
    }
}