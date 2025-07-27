using _2_Scripts.Enemies.FSM;
using _2_Scripts.Global;
using UnityEngine;

namespace _2_Scripts.Enemies.Agents
{
    public class ChargerAgent : DynamicEnemyBase
    {
        private bool _playerSeenOnRight;
        private bool _chargeFinished;
        private CleanEnemyStateMachine _stateMachine;
        private GameObject _sight; //TODO imo better to serialize it and connect directly in editor

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new CleanEnemyStateMachine(this);
            _sight = transform.Find("Sight").gameObject;
        }

        public override void OnSight(Collider2D other)
        { 
            if (other.gameObject.layer == (int)Layers.Player)
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