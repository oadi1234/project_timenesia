using System;

namespace _2_Scripts.Player.model
{
    [Serializable]
    public enum UnlockableName
    {
        None = 0,
        Dash = 1,
        LongDash = 2,
        SpatialDash = 3,
        TimeGate = 4, //save anywhere
        WallJump = 5,
        SwimUnderwater = 6,
        SlowMotionFocus = 7, //debug only? Might not be fun mid actual game.
        MidairFocus = 8, // might be deleted later or turned baseline ability.
        ChargedSpell = 9, // prepared spells (not focus) can be charged to increase their damage
        DoubleJump = 10,
        NoSource = 100,
        Aether = 101,
        Kinesis = 102,
        Mind = 103,
        Rune = 104,
        Entropy = 105
    }
}
