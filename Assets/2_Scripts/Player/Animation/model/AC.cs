using UnityEngine;

//TODO think whether we want it as an external json file or hardcoded
namespace _2_Scripts.Player.Animation.model
{
    /*
     * Animation constants class
     */
    public static class AC
    {
        public const float SingleFrame = 1f / 12f;
        #region character_animation_hash

        public static readonly int Idle = Animator.StringToHash("sylvia_idle_l");
        public static readonly int Run = Animator.StringToHash("sylvia_run_l");
        public static readonly int RunStop = Animator.StringToHash("sylvia_run_stop_l");
        public static readonly int RunStart = Animator.StringToHash("sylvia_run_start_l");
        public static readonly int Dash = Animator.StringToHash("sylvia_dash_l");
        public static readonly int DashEnd = Animator.StringToHash("sylvia_dash_end_l");
        public static readonly int Descend = Animator.StringToHash("sylvia_descent_l");
        public static readonly int Ascend = Animator.StringToHash("sylvia_ascend_l");
        public static readonly int DoubleJump = Animator.StringToHash("sylvia_double_jump_l");
        public static readonly int HurtHeavy = Animator.StringToHash("sylvia_hurt_heavy_l");
        public static readonly int HurtLight = Animator.StringToHash("sylvia_hurt_light_l");
        public static readonly int Wallslide = Animator.StringToHash("sylvia_wallslide_l");

        //Cloak & hair
        public static readonly int RopeIdle = Animator.StringToHash("Idle");
        public static readonly int RopeStartMove = Animator.StringToHash("Start-move");
        public static readonly int RopeStopMove = Animator.StringToHash("Stop-move");
        public static readonly int RopeMove = Animator.StringToHash("Move");

        //None animation (empty sprite) for certain states
        public static readonly int None = Animator.StringToHash("none");

        #endregion

        #region weapon animations

        //Staves
        public static readonly int StaffAttackAirDown1 = Animator.StringToHash("sylvia_staff_air_attack_down_1");
        public static readonly int StaffAttackAirDown2 = Animator.StringToHash("sylvia_staff_air_attack_down_2");
        public static readonly int StaffAttackAir1 = Animator.StringToHash("sylvia_staff_air_attack_l_1");
        public static readonly int StaffAttackAir2 = Animator.StringToHash("sylvia_staff_air_attack_l_2");
        public static readonly int StaffAttack1 = Animator.StringToHash("sylvia_staff_attack_l_1");
        public static readonly int StaffAttack2 = Animator.StringToHash("sylvia_staff_attack_l_2");
        public static readonly int StaffAttackAirUp = Animator.StringToHash("sylvia_staff_air_attack_up");
        public static readonly int StaffWallslideAttack = Animator.StringToHash("sylvia_staff_wallslide_attack");
        public static readonly int StaffHeavyAttack = Animator.StringToHash("sylvia_staff_heavy_attack_l");
        public static readonly int SpellcastStaffBolt = Animator.StringToHash("sylvia_spellcast_staff_bolt_l");
        public static readonly int SpellcastStaffBoltUp = Animator.StringToHash("sylvia_spellcast_staff_bolt_up_l");

        public static readonly int SpellcastStaffBoltDown =
            Animator.StringToHash("sylvia_spellcast_staff_bolt_down_l");

        public static readonly int SpellcastStaffHeavy = Animator.StringToHash("sylvia_spellcast_staff_heavy_l");
        public static readonly int SpellcastStaffAoE = Animator.StringToHash("sylvia_spellcast_staff_aoe_l");
        public static readonly int StaffConcentration = Animator.StringToHash("sylvia_staff_concentration_l");

        #endregion

        #region hurt_blink_hashes

        //Blinking frames
        public static readonly int HurtBlink = Animator.StringToHash("player_hurt_blink");
        public static readonly int HurtNone = Animator.StringToHash("player_hurt_none");

        #endregion

        #region attack_effects

        public static readonly int StaffBasicSwoosh = Animator.StringToHash("staff_basic_swoosh");

        #endregion

        #region animation_durations

        public const float AscendDuration = 5f / 12f;
        public const float DashEndDuration = 5f / 12f;
        public const float DashDuration = 8f / 24f;
        public const float DescentDuration = 5f / 12f;
        public const float DoubleJumpDuration = 8f / 12f;
        public const float HeavyHurtDuration = 11f / 12f;
        public const float LightHurtDuration = 6f / 12f;
        public const float IdleDuration = 1f;
        public const float RunDuration = 8f / 12f;
        public const float RunStartDuration = 3f / 12f;
        public const float RunStopDuration = 5f / 12f;
        public const float StaffSpellcastAoeDuration = 1f;
        public const float StaffSpellcastBoltDuration = 7f / 12f;
        public const float StaffSpellcastHeavyDuration = 11f / 12f;
        public const float StaffAttackDuration = 6f / 12f;
        public const float StaffAttackUpDuration = 5f / 12f;
        public const float StaffConcentrationDuration = 6f / 12f;

        public const float StaffHeavyAttackDuration = 10f / 12f;

        //cloak durations
        public const float CloakStopMoveDuration = 10f / 12f;
        public const float CloakIdleDuration = 1f / 12f;
        public const float CloakStartMoveDuration = 10f / 12f;
        public const float CloakMoveDuration = 14f / 12f;

        #endregion

        #region animation_state_locks

        public const float DashEndStateLockDuration = 0f;
        public const float DashStateLockDuration = 8f / 24f;
        public const float DoubleJumpStateLockDuration = 0f;
        public const float HeavyHurtStateLockDuration = 11f / 12f;
        public const float LightHurtStateLockDuration = 6f / 12f;
        public const float StaffSpellcastAoeStateLockDuration = 1f;
        public const float StaffSpellcastBoltStateLockDuration = 7f / 12f;
        public const float StaffSpellcastHeavyStateLockDuration = 11f / 12f;
        public const float StaffAttackStateLockDuration = 4f / 12f;
        public const float StaffHeavyAttackStateLockDuration = 9f / 12f;

        #endregion
    }
}