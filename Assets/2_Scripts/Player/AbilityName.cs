using System;

[Serializable]
public enum AbilityName //TODO(?): refactor values so they start from 1, not 0 (as 0 is default value of int) + other enums
{
    DoubleJump = 0,
    Dash = 1,
    LongDash = 2,
    SpatialDash = 3,
    TimeGate = 4, //save anywhere
    WallJump = 5,
    SwimUnderwater = 6,
    SlowmotionFocus = 7, //debug only? Might not be fun mid actual game.
    MidairFocus = 8, // might be deleted later or turned baseline ability.
    ChargedSpell = 9 // prepared spells (not focus) can be charged to increase their damage
}
