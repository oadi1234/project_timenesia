using System.Collections.Generic;
using _2_Scripts.Player;
using _2_Scripts.Player.model;

namespace _2_Scripts.Global.SaveSystem.Player
{
    public class PlayerAbilitiesAndStats
    {
        #region ABILITIES
        public Dictionary<UnlockableName, bool> abilities = new();
        #endregion

        #region BASE_STATS
        public int MaxHealth { get; set; }
        //public int CurrentConcentrationSlots { get; set; }
        //public int MaxConcentrationSlots { get; set; } //unused for now.
        public int MaxEffort { get; set; }
        public int SpellCapacity { get; set; }
        #endregion

        #region CURRENCIES
        //might get moved to some other class. There is no need to provide this to classes like movementController.
        public int Coins { get; set; }
        #endregion

        public void UnlockAbility(UnlockableName unlockableName)
        {
            abilities.Add(unlockableName, true);
        }

        public bool GetAbilityFlag(UnlockableName unlockableName)
        {
            return abilities.GetValueOrDefault(unlockableName, false);
        }
    }
}
