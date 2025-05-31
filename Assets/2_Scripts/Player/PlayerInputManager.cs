using System;
using System.Collections;
using System.Collections.Generic;
using _2_Scripts.Global.Animation.Model;
using _2_Scripts.Player.Controllers;
using _2_Scripts.Player.model;
using _2_Scripts.Player.Statistics;
using _2_Scripts.UI.Animation.Model;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        [SerializeField] private PlayerMovementController playerMovementController;

        [SerializeField] private PlayerSpellController playerSpellController;
        [SerializeField] private PlayerEffort playerEffort;
        [SerializeField] private SpellInventory spellInventory;

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
        private bool concentrationMode;
        private bool inputReceived = false;
        private bool isSpellcasting = false;
        private bool adjustAngleMode = false;
        private EffortType inputEffortType = EffortType.Raw;
        private List<EffortType> spellIndex = new();
        private float chargeTime = 0f;
        private float chargeCooldown = 0f;
        private float angle = 0f;
        private float angleModeAdjustStrength = 3f;
        private const float ChargeInputDuration = 2f;

        public event Action Attacked;
        public event Action InputReceived;
        public event Action HeavyAttack;

        private IEnumerator blockInputCoroutine;
        private IEnumerator blockMovementSkillInputCoroutine;

        private void Update()
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
            {
                concentrationHeld = false;
                inputEffortType = EffortType.EndOfInput;
            }

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

            if (!concentrationMode)
            {
                // TODO left like this for now, even though it bypasses blockInput. Once spell controller is worked out
                //  logic will need to be adjusted accordingly.
                if (Input.GetButtonDown("Spell1"))
                {
                    CastHotkeySpell(spellInventory.GetSpellAtSlot(1));
                }

                if (Input.GetButtonUp("Spell2"))
                {
                    CastHotkeySpell(spellInventory.GetSpellAtSlot(2));
                }

                if (Input.GetButtonUp("Spell3"))
                {
                    CastHotkeySpell(spellInventory.GetSpellAtSlot(3));
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
            }

            if (Input.GetButtonDown("Dash"))
            {
                dashPressed = true;
                inputReceived = true;
            }

            if (concentrationMode)
            {
                if (Input.GetButtonDown("EffortInput1"))
                    inputEffortType = EffortType.Aether;
                if (Input.GetButtonDown("EffortInput2"))
                    inputEffortType = EffortType.Entropy;
                if (Input.GetButtonDown("EffortInput3"))
                    inputEffortType = EffortType.Kinesis;
                if (Input.GetButtonDown("EffortInput4"))
                    inputEffortType = EffortType.Mind;
                if (Input.GetButtonDown("EffortInput5"))
                    inputEffortType = EffortType.Rune;
            }

            if (isInputEnabled)
            {
                playerEffort.SpellInput(inputEffortType);
                playerSpellController.CastHotkeySpell(spellIndex, isSpellcasting);
            }
        }

        private void LateUpdate()
        {
            inputEffortType = EffortType.NoInput;
            isSpellcasting = false;
        }

        private void FixedUpdate()
        {
            if (isInputEnabled)
            {
                playerMovementController.Move(xInput);
                playerMovementController.Jump(jumpPressed, jumpKeyHold, isMoveSkillInputEnabled);
                if (isMoveSkillInputEnabled)
                    playerMovementController.Dash(dashPressed);

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

            if (adjustAngleMode)
            {
                angle += yInput * angleModeAdjustStrength;
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

        public float GetAngle()
        {
            return Mathf.Clamp(angle, -70f, playerMovementController.IsGrounded() ? 70f : 0f);
        }

        public void SetAdjustAngleMode(bool value)
        {
            adjustAngleMode = value;
        }

        public void SetAngleModeAdjustStrength(float value)
        {
            angleModeAdjustStrength = value;
        }

        public void SetConcentration(bool value)
        {
            concentrationMode = value;
        }

        public bool IsAngleMode()
        {
            return adjustAngleMode;
        }

        public void SetAngle(float value)
        {
            angle = value;
        }

        public bool IsConcentrating()
        {
            return isInputEnabled && concentrationHeld;
        }

        public bool IsInConcentrationMode()
        {
            return concentrationMode;
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
            yield return
                new WaitForSeconds(
                    seconds); // might need to be realTime, but if we are to use Time.scale then this takes it into account.
            isInputEnabled = true;
        }

        private IEnumerator BlockMoveSkillInputForSeconds(float seconds)
        {
            isMoveSkillInputEnabled = false;
            yield return
                new WaitForSeconds(
                    seconds); // might need to be realTime, but if we are to use Time.scale then this takes it into account.
            isMoveSkillInputEnabled = true;
        }

        private void ResetLogic()
        {
            jumpPressed = false;
            dashPressed = false;
            inputReceived = false;
            attacking = false;
        }

        private void CastHotkeySpell(List<EffortType> effortCombination)
        {
            inputReceived = true;
            isSpellcasting = true;
            spellIndex = effortCombination;
        }
    }
}