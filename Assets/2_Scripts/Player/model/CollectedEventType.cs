namespace _2_Scripts.Model
{
    public enum CollectedEventType
    {
        None,
        CoinCollected,
        DoubleJumpCollected,
        DashCollected,
        LongDashCollected,
        SpatialDashCollected,
        TimeGateCollected, //save anywhere
        WallJumpCollected,
        SwimUnderwaterCollected,
        SlowmotionFocusCollected, //debug only? Might not be fun mid actual game.
        MidAirFocusCollected, // might be deleted later or turned baseline ability.
        ChargeSpellCollected // prepared spells (not focus) can be charged to increase their damage
    }
}