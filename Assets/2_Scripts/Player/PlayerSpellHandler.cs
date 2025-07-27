using _2_Scripts.Player.Animation;
using _2_Scripts.Player.Animation.model;
using _2_Scripts.Player.Controllers;
using _2_Scripts.Player.model;
using _2_Scripts.Player.ScriptableObjects;
using _2_Scripts.Player.Spell;
using _2_Scripts.Player.Spell.weaponData;
using _2_Scripts.Player.Statistics;
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
        [FormerlySerializedAs("ropeLogicOverride")] [SerializeField] private RopeEvents ropeEvents;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerSpriteRotater playerSpriteRotater;

        private WeaponAnimationSpellOrigin weaponAnimationSpellOrigin;
        private bool loopProtection = false;

        protected override void Awake()
        {
            base.Awake();
            weaponAnimationSpellOrigin = new StaffSpellOrigin(); //TODO load from save data instead.
            playerSpellController.Spellcasted += _ => { loopProtection = false; };
        }

        private void Update()
        {
            CalculateDirection();
        }

        public void BuffCast()
        {
            if (loopProtection) return;
            Spellbook.Instance.direction.Set(Direction, 1, 1);
            playerSpellController.InvokeSpell();

            if (!loopProtection)
            {
                loopProtection = true;
            }
        }

        public void RopeSway()
        {
            ropeEvents.Sway(new Vector2(Direction * -1, 0));
        }

        public void BoltSpellCastUp()
        {
            Spellbook.Instance.direction = Vector3.up;
            Spellbook.Instance.originPoint = weaponAnimationSpellOrigin.GetForState(WeaponAnimationState.SpellBoltUp);
            Spellbook.Instance.originPoint.x *= Direction;
            playerSpellController.InvokeSpell();
        }

        public void BoltSpellCastDown()
        {
            Spellbook.Instance.direction = Vector3.down;
            Spellbook.Instance.originPoint = weaponAnimationSpellOrigin.GetForState(WeaponAnimationState.SpellBoltDown);
            Spellbook.Instance.originPoint.x *= Direction;
            playerSpellController.InvokeSpell();
        }

        public void BoltSpellCast()
        {
            Spellbook.Instance.direction.Set(Direction, 0, 0);
            Spellbook.Instance.originPoint = weaponAnimationSpellOrigin.GetForState(WeaponAnimationState.SpellBolt);
            Spellbook.Instance.originPoint.x *= Direction;
            playerAnimationStateHandler.SetDoMovementStates(false);
            playerSpellController.InvokeSpell();
        }

        public void Concentrate()
        {
            ropeEvents.Sway(new Vector2(Direction * -1, 0));
            playerInputManager.SetConcentration(true);
        }

        public void StartAngleMode()
        {
            playerInputManager.SetAdjustAngleMode(true);
        }

        public void HeavySpellCastLunge()
        {
            var direction = Quaternion.Euler(0,0, playerInputManager.GetAngle()) * Vector3.right;

            playerMovementController.AttackLunge(PlayerConstants.Instance.staffLungeMagnitude * direction.x,
                AC.StaffHeavySpellcastLungeTimer, direction.y * PlayerConstants.Instance.staffLungeMagnitude);
            playerInputManager.SetAngleModeAdjustStrength(0f);
            playerHealth.SetNoVisualsIFrames(AC.StaffHeavySpellcastLungeTimer * 2f);
        }

        public void HeavySpellCast()
        {
            Spellbook.Instance.direction = new Vector3(Direction, 1, 1);
            playerSpellController.InvokeSpell();
        }

        public void AoESpellCast()
        {
            Spellbook.Instance.direction = new Vector3(Direction, 1, 1);
            playerSpellController.InvokeSpell();
        }

        public void EndAngleMode()
        {
            playerInputManager.SetAdjustAngleMode(false);
            playerInputManager.SetAngle(0f);
            playerInputManager.SetAngleModeAdjustStrength(3f);
        }
    }
}