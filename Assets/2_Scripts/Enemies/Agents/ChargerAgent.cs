using System;
using _2___Scripts.Global;
using _2_Scripts.Enemies.States;
using UnityEngine;

namespace _2_Scripts.Enemies.Agents
{
    public class ChargerAgent : MovingEnemyBase
    {
        private bool _playerSeenOnRight;
        private bool _chargeFinished;
        private bool _playerSeen;
        private CircleCollider2D _sight;
        private HaltState _haltState;

        protected override void Awake()
        {
            base.Awake();
            
            LoadingChargeState.OnChargeFinished += LoadingChargeStateOnChargeFinished;
            _sight = GetComponent<CircleCollider2D>();
            
            InitializeFsm();
        }

        private void InitializeFsm()
        {
            _haltState = new HaltState(this);
            var chargeRight = new LoadingChargeState(this, 3f);
            var chargeLeft = new LoadingChargeState(this, -3f);
            
            At(_haltState, chargeLeft, PlayerSeenFromLeft(), MovingStateMachine);
            At(_haltState, chargeRight, PlayerSeenFromRight(), MovingStateMachine);
            
            //below can be ass well written as:
            // MovingStateMachine.AddAnyTransition(haltState, ChargeFinished());
            At(chargeRight, _haltState, ChargeFinished(), MovingStateMachine);
            At(chargeLeft, _haltState, ChargeFinished(), MovingStateMachine);
            
            MovingStateMachine.SetState(_haltState);
        }

        private void LoadingChargeStateOnChargeFinished()
        {
            _chargeFinished = true;
            _sight.enabled = true;
            MovingStateMachine.SetState(_haltState);
        }

        private Func<bool> ChargeFinished()
        {
            return () => _chargeFinished;
        }


        private Func<bool> PlayerSeenFromLeft()
        {
            return () => !_playerSeenOnRight && !_chargeFinished;
        }
        private Func<bool> PlayerSeenFromRight()
        {
            return () => _playerSeenOnRight && !_chargeFinished;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == (int)LayerNames.Player)
            {
                _sight.enabled = false;
                _chargeFinished = false;
                _playerSeenOnRight = collision.transform.position.x - transform.position.x > 0;
            }
        }
    }
}