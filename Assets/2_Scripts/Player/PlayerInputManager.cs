using System;
using System.Collections;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        public PlayerMovementController playerMovementController;
        
        public PlayerSpellController playerSpellController;

        private float xInput;
        private float yInput;
        private bool isInputEnabled = true;
        private bool isMoveSkillInputEnabled = true;
        private bool jumpPressed;
        private bool jumpKeyHold;
        private bool attackHeld = false;
        private bool attacking = false;
        private bool dashPressed;
        private bool concentrationHeld;
        private bool inputReceived = false;
        private bool isSpellcasting = false;
        private int spellIndex = -1;
        private float chargeTime = 0f;
        private float chargeCooldown = 0f;
        private const float ChargeInputDuration = 2f;

        public event Action Attacked;
        public event Action InputReceived;
        public event Action HeavyAttack;
        
        private IEnumerator blockInputCoroutine;
        private IEnumerator blockMovementSkillInputCoroutine;

        // Update is called once per frame
        void Update()
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");

            if (attackHeld && chargeCooldown <= 0f)
                chargeTime += Time.deltaTime;
            if (chargeCooldown > 0f)
                chargeCooldown -= Time.deltaTime;
            
            
            if (Input.GetButtonDown("Concentration"))
                concentrationHeld = true;
            if (Input.GetButtonUp("Concentration"))
                concentrationHeld = false;

            if (Input.GetAxisRaw("Horizontal") != 0f)
            {
                inputReceived = true;
            }

            if (Input.GetButtonDown("Jump"))
            {
                jumpPressed = true;
                jumpKeyHold = true;
                inputReceived = true;
            }
            else if (Input.GetButtonUp("Jump"))
            {
                jumpKeyHold = false;
            }

            // TODO left like this for now, even though it bypasses blockInput. Once spell controller is worked out
            //  logic will need to be adjusted accordingly.
            if (Input.GetButtonDown("Spell1"))
            {
                CastSpell(0);
                 //TODO this might need some tweaking as to what is being sent
                // also for now the PlayerAnimationStateHandler handles this event, but in the future it should be rerouted
                // through some spellhandler class like thing, like the PlayerSpellController here.
            }

            if (Input.GetButtonUp("Spell2"))
            {
                CastSpell(1);
            }

            if (Input.GetButtonUp("Spell3"))
            {
                CastSpell(2);
            }

            if (Input.GetButtonDown("Attack"))
            {
                attackHeld = true;
                attacking = true;
                inputReceived = true;
            }

            if (Input.GetButtonUp("Attack"))
            {

                chargeTime = 0f;
                attackHeld = false;
            }

            if (Input.GetButtonDown("Dash"))
            {
                dashPressed = true;
                inputReceived = true;
            }
        }

        private void FixedUpdate()
        {
            if (isInputEnabled)
            {
                playerMovementController.Move(xInput * Time.fixedDeltaTime * 50);
                playerMovementController.Jump(jumpPressed, jumpKeyHold, isMoveSkillInputEnabled);
                if (isMoveSkillInputEnabled)
                    playerMovementController.Dash(dashPressed, xInput);
                playerSpellController.CastSpell(spellIndex, isSpellcasting);
                if (inputReceived)
                {
                    InputReceived?.Invoke();
                }

                // TODO move logic like action invocation to player attack controller
                if (attacking)
                {
                    if (chargeTime >= ChargeInputDuration)
                        HeavyAttack?.Invoke();
                    else
                        Attacked?.Invoke();
                }
            }
            else
            {
                xInput = 0;
            }

            ResetLogic();
        }

        public void SetInputEnabled(bool isEnabled)
        {
            isInputEnabled = isEnabled;
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
            return isInputEnabled && concentrationHeld;
        }

        public void SetChargeCooldown(float seconds)
        {
            chargeCooldown = seconds;
        }

        public void BlockInput(float seconds)
        {
            blockInputCoroutine = BlockInputForSeconds(seconds);
            StartCoroutine(blockInputCoroutine);
        }

        public void BlockMovementSkillInput(float seconds)
        {
            blockMovementSkillInputCoroutine = BlockMoveSkillInputForSeconds(seconds);
            StartCoroutine(blockMovementSkillInputCoroutine);
        }

        public void StopBlockInput()
        {
            StopCoroutine(blockInputCoroutine);
        }
        
        // moved from playerMovementController to here, since it makes more sense for input related things to be handled here.
        private IEnumerator BlockInputForSeconds(float seconds)
        {
            isInputEnabled = false;
            yield return new WaitForSeconds(seconds); // might need to be realTime, but if we are to use Time.scale then this takes it into account.
            isInputEnabled = true;
        }
        
        private IEnumerator BlockMoveSkillInputForSeconds(float seconds)
        {
            isMoveSkillInputEnabled = false;
            yield return new WaitForSeconds(seconds); // might need to be realTime, but if we are to use Time.scale then this takes it into account.
            isMoveSkillInputEnabled = true;
        }

        private void ResetLogic()
        {
            jumpPressed = false;
            dashPressed = false;
            inputReceived = false;
            isSpellcasting = false;
            attacking = false;
            spellIndex = -1;
        }

        private void CastSpell(int index)
        {
            inputReceived = true;
            isSpellcasting = true;
            spellIndex = index;
        }
    }
}