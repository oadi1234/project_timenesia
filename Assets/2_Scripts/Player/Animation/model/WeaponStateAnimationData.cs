using System.Collections.Generic;

namespace _2_Scripts.Player.Animation.model
{
    public class WeaponStateAnimationData
    {
        protected Dictionary<WeaponAnimationState, AnimationData> AnimationPerWeapon;
        private static readonly AnimationData NoAnim = new AnimationData(AC.None, 0, 0);

        protected WeaponStateAnimationData()
        {
            //TODO the data here should load depending on savefile data.
            // I feel like it is better to do it dynamically like this due to the fact that we are going to have multiple
            // different weapons, even if they are the same type.
            // Depending on the needs we can switch out data contained in this class without affecting logic.
            //TODO "if not new game load save, else default data"
        }

        public AnimationData GetForState(WeaponAnimationState state)
        {
            return AnimationPerWeapon.GetValueOrDefault(state, NoAnim);
        }

        public AnimationData GetLastInChain(WeaponAnimationState state)
        {
            var data = AnimationPerWeapon.GetValueOrDefault(state, NoAnim);
            while (data.chainsInto != null)
            {
                data = data.chainsInto;
            }

            return data;
        }
    }
}