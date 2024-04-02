using _2___Scripts.Global;
using UnityEngine;

namespace _2_Scripts.Enemies.Temp_SecondApproach
{
    public class DifferentApproachChargerAgent : EnemyBase
    {
        private bool _playerSeenOnRight;
        private bool _chargeFinished;
        private CleanEnemyStateMachine _stateMachine;
        private CircleCollider2D _sight;

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new CleanEnemyStateMachine(this);
            _sight = GetComponent<CircleCollider2D>();
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == (int)LayerNames.Player)
            {
                _sight.enabled = false;
                _stateMachine.ChangeState(
                    _playerSeenOnRight == collision.transform.position.x - transform.position.x < 0
                        ? _stateMachine.RightChargeState
                        : _stateMachine.LeftChargeState);
            }
            ChargeState.OnChargeFinished += LoadingChargeStateOnChargeFinished;
        }

        private void LoadingChargeStateOnChargeFinished()
        {
            _sight.enabled = true;
        }

        private void FixedUpdate()
        {
            _stateMachine.OnUpdate();
        }
    }
}