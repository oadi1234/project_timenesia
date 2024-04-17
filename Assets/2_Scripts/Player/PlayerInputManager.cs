using _2_Scripts.Model;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        public PlayerMovementController playerMovementController;
        public PlayerAttackController playerAttackController;
        public WeaponController weaponController;

        float _xInput = 0f;
        float _yInput = 0f;
        float _yInputForCombos = 0f;
        bool _isInputEnabled = true;
        bool _jumpPressed = false;
        bool _jumpKeyHold = false;
        bool _spellAttack = false;
        bool _weaponAttack = false;
        bool _dashPressed = false;
        int _spellIndex = -1;

        private bool _castingAnimationInProgress = false;
        public bool IsInputEnabled => _isInputEnabled;

        // Update is called once per frame
        void Update()
        {
            _xInput = Input.GetAxisRaw("Horizontal");
            _yInput = Input.GetAxisRaw("Vertical");
            if (Input.GetButtonDown("Jump"))
            {
                _jumpPressed = true;
                _jumpKeyHold = true;
            }
            else if (Input.GetButtonUp("Jump"))
            {
                _jumpKeyHold = false;
            }

            if (Input.GetButtonDown("Fire1") && !_castingAnimationInProgress)
            {
                _spellAttack = true;
                _spellIndex = 0;
            }
            if (Input.GetButtonDown("Fire2") && !_castingAnimationInProgress)
            {
                _spellAttack = true;
                _spellIndex = 1;
            }

            if (Input.GetButtonDown("Fire3"))
            {
                _yInputForCombos = _yInput;
                _weaponAttack = true;
            }
            if(Input.GetButtonDown("Dash"))
            {
                _dashPressed = true;
            }

            if (Input.GetButtonDown("SwitchWeapon"))
            {
                weaponController.QuickChangeWeapon();
            }
        }

        private void FixedUpdate()
        {
            if (_isInputEnabled)
            { 
                if (_spellAttack)
                {
                    playerAttackController.Attack(_spellIndex);
                    _castingAnimationInProgress = true;
                    _spellAttack = false;
                    _spellIndex = -1;
                }
                else if (_weaponAttack)
                {
                    weaponController.Attack(GetDirectionOfCombo());
                    _weaponAttack = false;
                }
                else
                {
                    //time.fixeddeltatime below is used to adjust movement for occasional lag, if it ever happens. It might be unnecessary.
                    //on normal run xInput*fixedDeltaTime is 0.02, so I multiply it by 50 to normalize it to 1. Makes movement speed easier to set in constants.
                    // TODO: This will most likely require adjustment if a time slow effects are implemented.
                    playerMovementController.Move(_xInput * Time.fixedDeltaTime * 50);
                    playerMovementController.Jump(_jumpPressed, _jumpKeyHold);
                    playerMovementController.Dash(_dashPressed, _xInput);

                    _jumpPressed = false;
                    _dashPressed = false;
                }
            }
            else
            {
                _xInput = 0;
                _jumpKeyHold = false;
                _jumpPressed = false;
            }
        }

        private Direction GetDirectionOfCombo()
        {
            return _yInputForCombos == 0
                ? (playerMovementController.IsFacingLeft() ? Direction.LEFT : Direction.RIGHT)
                : _yInputForCombos > 0
                    ? Direction.UP
                    : Direction.DOWN;
        }

        public void SetInputEnabled(bool enable)
        {
            _isInputEnabled = enable;
        }

        public void CastingAnimationFinished()
        {
            _castingAnimationInProgress = false;
        }
    }
}
