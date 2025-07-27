namespace _2_Scripts.Global.Events.Model
{
    public enum CollectedEventType
    {
        None,
        
        #region abilities
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
        ChargeSpellCollected, // prepared spells (not focus) can be charged to increase their damage
        #endregion
        
        #region magic_sources
        AetherCollected, // starting magic source, but might be fun for modded randomizers in the future.
        KinesisCollected, // as above.
        MindCollected,
        RuneCollected,
        EntropyCollected,
        #endregion
        
        #region spells
        SpellCollected
        #endregion
    }
}