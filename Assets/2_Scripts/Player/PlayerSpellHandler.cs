using _2_Scripts.Player.Animation;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using _2_Scripts.Player.model;
using _2_Scripts.Player.ScriptableObjects;
using _2_Scripts.Player.Spell;
using _2_Scripts.Player.Spell.weaponData;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2_Scripts.Player
{
    public class PlayerSpellHandler : SpriteFacingDirection
    {
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private PlayerInputManager playerInputManager;
        [SerializeField] private PlayerSpellController playerSpellController;
        [SerializeField] private PlayerAnimationStateHandler playerAnimationStateHandler;
        [SerializeField] private RopeLogicOverride ropeLogicOverride;

        private WeaponAnimationSpellOrigin weaponAnimationSpellOrigin;
        private bool loopProtection = false;

        public override void Awake()
        {
            base.Awake();
            weaponAnimationSpellOrigin = new StaffSpellOrigin(); //TODO load from save data instead.
            playerSpellController.Spellcasted += _ => { loopProtection = false;};
        }

        private void Update()
        {
            CalculateDirection();
        }

        public void BuffCast()
        {
            //prevent looping animation from triggering spellcast again.
            if (loopProtection) return;
            Spellbook.Instance.direction.Set(Direction, 1,1);
            playerSpellController.InvokeSpell();
            
            if (!loopProtection)
            {
                loopProtection = true;
            }
        }

        public void RopeSway()
        {
            ropeLogicOverride.Sway(new Vector2(Direction * -1, 0));
        }

        public void BoltSpellCastUp()
        {
            Spellbook.Instance.direction = Vector3.up;
            Spellbook.Instance.originPoint=weaponAnimationSpellOrigin.GetForState(WeaponAnimationState.SpellBoltUp);
            Spellbook.Instance.originPoint.x *= Direction;
            playerSpellController.InvokeSpell();
        }
        
        public void BoltSpellCastDown()
        {
            Spellbook.Instance.direction = Vector3.down;
            Spellbook.Instance.originPoint=weaponAnimationSpellOrigin.GetForState(WeaponAnimationState.SpellBoltDown);
            Spellbook.Instance.originPoint.x *= Direction;
            playerSpellController.InvokeSpell();
        }
        
        public void BoltSpellCast()
        {
            Spellbook.Instance.direction.Set(Direction, 0, 0);
            Spellbook.Instance.originPoint=weaponAnimationSpellOrigin.GetForState(WeaponAnimationState.SpellBolt);
            Spellbook.Instance.originPoint.x *= Direction;
            playerAnimationStateHandler.SetDoMovementStates(false);
            playerSpellController.InvokeSpell();
        }

        public void Concentrate()
        {
            ropeLogicOverride.Sway(new Vector2(Direction * -1, 0));
            playerInputManager.SetConcentration(true);
        }

        public void StartAngleMode()
        {
            playerInputManager.SetAdjustAngleMode(true);
        }

        public void DoLungeOnHeavySpellcast()
        {
            var angle = playerInputManager.GetAngle();
            var direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

            playerMovementController.AttackLunge(PlayerConstants.Instance.staffLungeMagnitude * direction.x,
                AC.StaffHeavySpellcastLungeTimer, direction.y * PlayerConstants.Instance.staffLungeMagnitude);
            playerInputManager.SetAngleModeAdjustStrength(1f);
        }

        public void EndAngleMode()
        {
            playerInputManager.SetAdjustAngleMode(false);
            playerInputManager.SetAngle(0f);
            playerInputManager.SetAngleModeAdjustStrength(3f);
        }
    }
}