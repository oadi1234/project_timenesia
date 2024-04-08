using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityAndStats
{

    #region ABILITIES
    public Dictionary<AbilityName, bool> abilities = new();
    #endregion

    #region BASE_STATS
    //might get moved to some other class. There is no need to provide this to classes like movementController.
    public int CurrentHealth { get; set; }
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

    public void UnlockAbility(AbilityName abilityName)
    {
        abilities.Add(abilityName, true);
    }

    public bool GetAbilityFlag(AbilityName abilityName)
    {
        return abilities.GetValueOrDefault(abilityName, false);
    }

}
