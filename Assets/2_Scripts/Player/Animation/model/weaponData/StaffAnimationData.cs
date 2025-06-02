using System.Collections.Generic;

namespace _2_Scripts.Player.Animation.model.weaponData
{
    //classes in this package might be converted to an external file
    public class StaffAnimationData : WeaponStateAnimationData
    {
        public StaffAnimationData()
        {
            var groundAttack1 =
                new AnimationData(AC.StaffAttack1, AC.StaffAttackDuration, AC.StaffAttackStateLockDuration);
            var groundAttack2 =
                new AnimationData(AC.StaffAttack2, AC.StaffAttackDuration, AC.StaffAttackStateLockDuration);

            groundAttack1.SetChainsInto(ref groundAttack2);

            var airAttack1 = new AnimationData(AC.StaffAttackAir1, AC.StaffAttackDuration,
                AC.StaffAttackStateLockDuration);
            var airAttack2 = new AnimationData(AC.StaffAttackAir2, AC.StaffAttackDuration,
                AC.StaffAttackStateLockDuration);
            airAttack1.SetChainsInto(ref airAttack2);

            var attackDown1 = new AnimationData(AC.StaffAttackAirDown1, AC.StaffAttackDuration,
                AC.StaffAttackStateLockDuration);
            var attackDown2 = new AnimationData(AC.StaffAttackAirDown2, AC.StaffAttackDuration,
                AC.StaffAttackStateLockDuration);
            attackDown1.SetChainsInto(ref attackDown2);

            var buffShortStart = new AnimationData(AC.StaffConcentrationStart, AC.StaffConcentrationStartDuration, AC.StaffConcentrationStartDuration);
            var buffShortOngoing = new AnimationData(AC.StaffBuffOngoing, AC.BuffShortDuration, AC.BuffShortDuration);
            var buffEnd = new AnimationData(AC.StaffBuffEnd, AC.BuffEndDuration, AC.StaffBuffEndStateLockDuration);
            buffShortOngoing.SetChainsInto(ref buffEnd);
            buffShortStart.SetChainsInto(ref buffShortOngoing);

            var concentrationStart = new AnimationData(AC.StaffConcentrationStart, AC.StaffConcentrationStartDuration, 0f);
            var concentrationOngoing = new AnimationData(AC.StaffConcentration, AC.StaffConcentrationDuration, 0f);
            var concentrationEnd = new AnimationData(AC.StaffConcentrationEnd, AC.StaffConcentrationEndDuration, 0f);
            concentrationOngoing.SetChainsInto(ref concentrationEnd);
            concentrationStart.SetChainsInto(ref concentrationOngoing);

            AnimationPerWeapon = new Dictionary<WeaponAnimationState, AnimationData>()
            {
                { WeaponAnimationState.AttackGround, groundAttack1 },
                { WeaponAnimationState.AttackAir, airAttack1 },
                { WeaponAnimationState.SpellShortBuff, buffShortStart },
                { WeaponAnimationState.Concentration, concentrationStart },
                {
                    WeaponAnimationState.AttackUp,
                    new AnimationData(AC.StaffAttackAirUp, AC.StaffAttackUpDuration, AC.StaffAttackStateLockDuration)
                },
                { WeaponAnimationState.AttackDown, attackDown1 },
                {
                    WeaponAnimationState.HeavyAttack,
                    new AnimationData(AC.StaffHeavyAttack, AC.StaffHeavyAttackDuration,
                        AC.StaffHeavyAttackStateLockDuration)
                },
                {
                    WeaponAnimationState.SpellBolt,
                    new AnimationData(AC.SpellcastStaffBolt, AC.StaffSpellcastBoltDuration,
                        AC.StaffSpellcastBoltStateLockDuration)
                },
                {
                    WeaponAnimationState.SpellBoltDown,
                    new AnimationData(AC.SpellcastStaffBoltDown, AC.StaffSpellcastBoltDuration,
                        AC.StaffSpellcastBoltStateLockDuration)
                },
                {
                    WeaponAnimationState.SpellBoltUp,
                    new AnimationData(AC.SpellcastStaffBoltUp, AC.StaffSpellcastBoltDuration,
                        AC.StaffSpellcastBoltStateLockDuration)
                },
                {
                    WeaponAnimationState.ConcentrationStart,
                    new AnimationData(AC.StaffConcentrationStart, AC.StaffConcentrationStartDuration, 0f)
                },
                {
                    WeaponAnimationState.SpellHeavy,
                    new AnimationData(AC.SpellcastStaffHeavy, AC.StaffSpellcastHeavyDuration,
                        AC.StaffSpellcastHeavyStateLockDuration)
                },
                {
                    WeaponAnimationState.SpellAoE,
                    new AnimationData(AC.SpellcastStaffAoE, AC.StaffSpellcastAoeDuration,
                        AC.StaffSpellcastAoeStateLockDuration)
                },
                {
                    WeaponAnimationState.Wallsliding,
                    new AnimationData(AC.StaffWallslideAttack, AC.StaffAttackDuration, AC.StaffAttackStateLockDuration)
                }
            };
        }
    }
}