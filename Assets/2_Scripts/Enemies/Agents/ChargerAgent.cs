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

        protected override void Awake()
        {
            base.Awake();
            
            LoadingChargeState.OnChargeFinished += LoadingChargeStateOnChargeFinished;
            _sight = GetComponent<CircleCollider2D>();
            
            InitializeFsm();
        }

        private void InitializeFsm()
        {
            var haltState = new HaltState(this);
            var chargeRight = new LoadingChargeState(this, 3f);
            var chargeLeft = new LoadingChargeState(this, -3f);
            
            At(haltState, chargeLeft, PlayerSeenFromLeft(), MovingStateMachine);
            At(haltState, chargeRight, PlayerSeenFromRight(), MovingStateMachine);
            
            //below can be ass well written as:
            // MovingStateMachine.AddAnyTransition(haltState, ChargeFinished());
            At(chargeRight, haltState, ChargeFinished(), MovingStateMachine);
            At(chargeLeft, haltState, ChargeFinished(), MovingStateMachine);
            
            MovingStateMachine.SetState(haltState);
        }

        private void LoadingChargeStateOnChargeFinished()
        {
            _chargeFinished = true;
            _sight.enabled = true;
            _playerSeen = false;
        }

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
            return () => _playerSeen &&_playerSeenOnRight && !_chargeFinished;
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
    }
}