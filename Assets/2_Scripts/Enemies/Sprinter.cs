using System;
using _2___Scripts.Global;
using _2_Scripts.Enemies.States;
using UnityEngine;

namespace _2_Scripts.Enemies
{
    public class Sprinter : MovingEnemyBase
    {
        private bool _playerSeen;
        private bool _playerSeenOnRight;
        private bool _chargeFinished;
        private CircleCollider2D _sight;

        protected override void Awake()
        {
            base.Awake();
            // var patrolingState = new JumpInPlaceState(this);
            var haltState = new HaltState(this);
            var chargeRight = new LoadingChargeState(this, 3f);
            var chargeLeft = new LoadingChargeState(this, -3f);
            At(haltState, chargeLeft, PlayerSeenFromLeft(), MovingStateMachine);
            At(haltState, chargeRight, PlayerSeenFromRight(), MovingStateMachine);
            At(chargeRight, haltState, ChargeFinished(), MovingStateMachine);
            At(chargeLeft, haltState, ChargeFinished(), MovingStateMachine);
            // MovingStateMachine.AddAnyTransition(haltState, PlayerNotSeen());
            
            MovingStateMachine.SetState(haltState);
            
            LoadingChargeState.chargeFinished += LoadingChargeStateOnchargeFinished;
            _sight = GetComponent<CircleCollider2D>();
        }

        private void LoadingChargeStateOnchargeFinished(bool c)
        {
            _chargeFinished = c;
            _sight.enabled = true;

            // _playerSeenOnRight = PlayerPosition.GetPlayerPosition().position.x - transform.position.x > 0;
        }

        // new void Update()
        // {
        //     base.Update();
        //     // Debug.Log(_playerSeen + "  " + _playerSeenOnRight);
        // }
        
        


        private Func<bool> ChargeFinished()
        {
            return () => _chargeFinished;
        }


        private Func<bool> PlayerSeenFromLeft()
        {
            return () => _playerSeen && !_playerSeenOnRight && !_chargeFinished;
        }
        private Func<bool> PlayerSeenFromRight()
        {
            return () => _playerSeen && _playerSeenOnRight && !_chargeFinished;
        }


        private Func<bool> PlayerSeen()
        {
            return () => _playerSeen;
        }

        private Func<bool> PlayerNotSeen()
        {
            return () => !_playerSeen;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == (int)LayerNames.Player)
            {
                _sight.enabled = false;
                _chargeFinished = false;
                _playerSeen = true;
                _playerSeenOnRight = collision.transform.position.x - transform.position.x > 0;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == (int)LayerNames.Player)
                _playerSeen = false;
        }
    }
}