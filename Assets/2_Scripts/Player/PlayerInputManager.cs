using System;
using _2_Scripts.Player.Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2_Scripts.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        public PlayerMovementController playerMovementController;

        [FormerlySerializedAs("playerAttackController")]
        public PlayerSpellController playerSpellController;

        public WeaponController weaponController;

        private float xInput;
        private float yInput;
        private bool isInputEnabled = true;
        private bool jumpPressed;
        private bool jumpKeyHold;
        private bool spellAttack;
        private bool attackHeld = false;
        private bool dashPressed;
        private bool concentrationHeld;
        private int spellIndex = -1;
        private float chargeTime = 0f;
        private float chargeCooldown = 0f;
        private const float ChargeInputDuration = 2f;

        public event Action Attacked;
        public event Action InputReceived;
        public event Action HeavyAttack;
        public event Action<int> Spellcasted;

        private bool _castingAnimationInProgress;
        public bool IsInputEnabled => isInputEnabled;

        // Update is called once per frame
        void Update()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");
            if (Input.GetButtonDown("Concentration"))
                concentrationHeld = true;
            if (Input.GetButtonUp("Concentration"))
                concentrationHeld = false;

            if (attackHeld && chargeCooldown <= 0f)
                chargeTime += Time.deltaTime;
            if (chargeCooldown > 0f)
                chargeCooldown -= Time.deltaTime;

            if (Input.GetAxisRaw("Horizontal") != 0f)
            {
                InputReceived?.Invoke();
            }

            if (Input.GetButtonDown("Jump"))
            {
                jumpPressed = true;
                jumpKeyHold = true;
                InputReceived?.Invoke();
            }
            else if (Input.GetButtonUp("Jump"))
            {
                jumpKeyHold = false;
            }

            if (Input.GetButtonDown("Spell1") && !_castingAnimationInProgress)
            {
                // spellAttack = true;
                // spellIndex = 0;
                InputReceived?.Invoke();
                Spellcasted?.Invoke(1); //TODO this might need some tweaking as to what is being sent
                // also for now the PlayerAnimationStateHandler handles this event, but in the future it should be rerouted
                // through some spellhandler class like thing, like the PlayerSpellController here.
            }

            if (Input.GetButtonUp("Spell2"))
            {
                Spellcasted?.Invoke(2);
            }
            
            if (Input.GetButtonUp("Spell3"))
            {
                Spellcasted?.Invoke(3);
                
            }

            if (Input.GetButtonDown("Attack"))
            {
                attackHeld = true;
                Attacked?.Invoke();
                InputReceived?.Invoke();
            }

            if (Input.GetButtonUp("Attack"))
            {
                if (chargeTime >= ChargeInputDuration)
                {
                    HeavyAttack?.Invoke();
                }

                chargeTime = 0f;
                attackHeld = false;
            }

            if (Input.GetButtonDown("Dash"))
            {
                dashPressed = true;
                InputReceived?.Invoke();
            }


            // if (Input.GetButtonDown("SwitchWeapon"))
            // {
            //     weaponController.QuickChangeWeapon();
            // }
        }

        private void FixedUpdate()
        {
            if (isInputEnabled)
            {
                // if (spellAttack)
                // {
                    // playerSpellController.Attack(spellIndex);
                    // _castingAnimationInProgress = true;
                    // spellAttack = false;
                    // spellIndex = -1;
                    //TODO the above stuff works, but since an event is a convenient thing for triggering animations
                    // we could simply use those to trigger stuff in the spell controller
                // }
                // else
                // {
                    playerMovementController.Move(xInput * Time.fixedDeltaTime * 50);
                    playerMovementController.Jump(jumpPressed, jumpKeyHold);
                    playerMovementController.Dash(dashPressed, xInput);

                    jumpPressed = false;
                    dashPressed = false;
                // }
            }
            else
            {
                xInput = 0;
                jumpKeyHold = false;
                jumpPressed = false;
            }
        }

        public void SetInputEnabled(bool isEnabled)
        {
            isInputEnabled = isEnabled;
        }

        public void CastingAnimationFinished()
        {
            _castingAnimationInProgress = false;
        }

        public float GetXInput()
        {
            return xInput;
        }

        public float GetYInput()
        {
            return yInput;
        }

        public bool IsConcentrating()
        {
            return concentrationHeld;
        }

        public void SetChargeCooldown(float seconds)
        {
            chargeCooldown = seconds;
        }
    }
}