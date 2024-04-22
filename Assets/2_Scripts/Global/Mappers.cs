using System;
using _2_Scripts.Model;

namespace _2_Scripts.Global
{
    public static class Mappers
    {
        public static AbilityName Map(CollectedEventType eventType)
        {
            return eventType switch
            {
                CollectedEventType.DoubleJumpCollected => AbilityName.DoubleJump,
                CollectedEventType.DashCollected => AbilityName.Dash,
                CollectedEventType.LongDashCollected => AbilityName.LongDash,
                CollectedEventType.SpatialDashCollected => AbilityName.SpatialDash,
                CollectedEventType.TimeGateCollected => AbilityName.TimeGate,
                CollectedEventType.WallJumpCollected => AbilityName.WallJump,
                CollectedEventType.SwimUnderwaterCollected => AbilityName.SwimUnderwater,
                CollectedEventType.SlowmotionFocusCollected => AbilityName.SlowMotionFocus,
                CollectedEventType.MidAirFocusCollected => AbilityName.MidairFocus,
                CollectedEventType.ChargeSpellCollected => AbilityName.ChargedSpell,
                _ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null)
            };
        }
    }
}