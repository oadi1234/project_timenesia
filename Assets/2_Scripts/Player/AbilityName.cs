using System;

[Serializable]
public enum AbilityName
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
