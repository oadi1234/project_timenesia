using _2___Scripts.Global;
using UnityEngine;

namespace _2_Scripts.Enemies.Temp_SecondApproach
{
    public class DifferentApproachChargerAgent : DynamicEnemyBase
    {
        private bool _playerSeenOnRight;
        private bool _chargeFinished;
        private CleanEnemyStateMachine _stateMachine;
        // private Collider2D _body;
        private GameObject _sight;

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new CleanEnemyStateMachine(this);
            // _body = GetComponent<Collider2D>();
            _sight = transform.Find("Sight").gameObject;
        }

        public override void OnSight(Collider2D other)
        { 
            if (other.gameObject.layer == (int)LayerNames.Player)
            {
                SetSightEnabled(false);
                _stateMachine.ChangeState(
                    _playerSeenOnRight == other.transform.position.x - transform.position.x < 0
                        ? _stateMachine.RightChargeState
                        : _stateMachine.LeftChargeState);
            }
        }

        public override void SetSightEnabled(bool sightEnabled)
        {
            _sight.SetActive(sightEnabled);
        }
        
        private void FixedUpdate()
        {
            _stateMachine.OnUpdate();
        }
    }
}