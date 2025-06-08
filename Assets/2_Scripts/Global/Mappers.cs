using System;
using _2_Scripts.Global.Events.Model;
using _2_Scripts.Player.model;

namespace _2_Scripts.Global
{
    public static class Mappers
    {
        public static UnlockableName Map(CollectedEventType eventType)
        {
            return eventType switch
            {
                CollectedEventType.DoubleJumpCollected => UnlockableName.DoubleJump,
                CollectedEventType.DashCollected => UnlockableName.Dash,
                CollectedEventType.LongDashCollected => UnlockableName.LongDash,
                CollectedEventType.SpatialDashCollected => UnlockableName.SpatialDash,
                CollectedEventType.TimeGateCollected => UnlockableName.TimeGate,
                CollectedEventType.WallJumpCollected => UnlockableName.WallJump,
                CollectedEventType.SwimUnderwaterCollected => UnlockableName.SwimUnderwater,
                CollectedEventType.SlowmotionFocusCollected => UnlockableName.SlowMotionFocus,
                CollectedEventType.MidAirFocusCollected => UnlockableName.MidairFocus,
                CollectedEventType.ChargeSpellCollected => UnlockableName.ChargedSpell,
                _ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null)
            };
        }
    }
}