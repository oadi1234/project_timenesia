using System;
using _2_Scripts.Player.Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace _2_Scripts.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        public PlayerMovementController playerMovementController;
        [FormerlySerializedAs("playerAttackController")] public PlayerSpellController playerSpellController;
        public WeaponController weaponController;

        float xInput = 0f;
        float yInput = 0f;
        float yInputForCombos = 0f;
        bool isInputEnabled = true;
        bool jumpPressed = false;
        bool jumpKeyHold = false;
        bool spellAttack = false;
        bool weaponAttack = false;
        bool dashPressed = false;
        int spellIndex = -1;

        public event Action Attacked;
        public event Action InputReceived;

        private bool _castingAnimationInProgress = false;
        public bool IsInputEnabled => isInputEnabled;

        // Update is called once per frame
        void Update()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");
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

            if (Input.GetButtonDown("Fire1") && !_castingAnimationInProgress)
            {
                spellAttack = true;
                spellIndex = 0;
                InputReceived?.Invoke();
            }
            if (Input.GetButtonDown("Fire2") && !_castingAnimationInProgress)
            {
                spellAttack = true;
                spellIndex = 1;
                InputReceived?.Invoke();
            }

            if (Input.GetButtonDown("Attack"))
            {
                yInputForCombos = yInput;
                // weaponAttack = true;
                Attacked?.Invoke();
                InputReceived?.Invoke();
            }
            if(Input.GetButtonDown("Dash"))
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
                if (spellAttack)
                {
                    playerSpellController.Attack(spellIndex);
                    _castingAnimationInProgress = true;
                    spellAttack = false;
                    spellIndex = -1;
                }
                // else if (weaponAttack) //Disabled because it messed with animations
                // {
                //     weaponController.Attack(GetDirectionOfCombo());
                //     weaponAttack = false;
                // }
                else
                {
                    //time.fixeddeltatime below is used to adjust movement for occasional lag, if it ever happens. It might be unnecessary.
                    //on normal run xInput*fixedDeltaTime is 0.02, so I multiply it by 50 to normalize it to 1. Makes movement speed easier to set in constants.
                    // TODO: This will most likely require adjustment if a time slow effects are implemented.
                    playerMovementController.Move(xInput * Time.fixedDeltaTime * 50);
                    playerMovementController.Jump(jumpPressed, jumpKeyHold);
                    playerMovementController.Dash(dashPressed, xInput);

                    jumpPressed = false;
                    dashPressed = false;
                }
            }
            else
            {
                xInput = 0;
                jumpKeyHold = false;
                jumpPressed = false;
            }
        }

        private Direction GetDirectionOfCombo()
        {
            return yInputForCombos == 0
                ? (playerMovementController.IsFacingLeft() ? Direction.LEFT : Direction.RIGHT)
                : yInputForCombos > 0
                    ? Direction.UP
                    : Direction.DOWN;
        }

        public void SetInputEnabled(bool enable)
        {
            isInputEnabled = enable;
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
    }
}
