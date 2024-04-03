using System;
using System.Collections;
using _2___Scripts.Enemies.Attacks;
using _2_Scripts.Enemies.Attacks;
using _2_Scripts.Enemies.Temp_FirstApproach;
using Assets.Scripts.Enemies.States;
using UnityEngine;

namespace _2_Scripts.Enemies
{
    internal class Fungy : MonoBehaviour
    {
        private StateMachine _movingStateMachine;
        private StateMachine _attackingStateMachine;
        public GameObject _plum;

        public event Action<int> OnEnemyKilled;

        private bool _playerSeen = false;

        private Rigidbody2D _playerPosition;
        private SpriteRenderer _spriteRenderer;

        public Rigidbody2D PlayerPosition { get { return _playerPosition; } private set { } }

        private Rigidbody2D _rigidbody2D;
        public Rigidbody2D RigidBody { get { return _rigidbody2D; } private set { } }

        private float _timeInterval = 5f;
        private float _currentTimeInterval = 0f;

        private Color darkering = new Color(0.002f, 0.002f, 0, 0);

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _playerPosition = GameObject.Find("Player").GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            BaseAttack.Attack += Attack_AoE_OnAttackHit;

            var attackingState = new AttackState_Fungy(this);
            var patrolingState = new PatrolingState_Fungy(this);

            InitializeEnemy(patrolingState);

            At(patrolingState, attackingState, PlayerSeen(), _movingStateMachine);
            At(attackingState, patrolingState, PlayerNotSeen(), _movingStateMachine);

            void At(IState from, IState to, Func<bool> condition, StateMachine stateMachine) => stateMachine.AddTransition(from, to, condition);

            //Remember: SetFirstStateAlwaysAfterAddingOtherStates!
            _movingStateMachine.SetState(patrolingState);

        }

        private void Attack_AoE_OnAttackHit(BaseAttack obj)
        {
            _plum.SetActive(false);
        }

        internal void FlipSprite(bool flip)
        {
            _spriteRenderer.flipX = flip;
        }

        private Func<bool> PlayerSeen()
        {
            return () => _playerSeen;
        }

        private Func<bool> PlayerNotSeen()
        {
            return () => !_playerSeen;
        }

        private void InitializeEnemy(IState firstState)
        {
            _movingStateMachine = new StateMachine();
            _attackingStateMachine = new StateMachine();
        }
        private void Update()
        {
            // Debug.Log(_playerSeen);
            _movingStateMachine.OnLogic();
            _attackingStateMachine.OnLogic();
            PrepareFire();
        }

        private void OnDestroy()
        {
            OnEnemyKilled?.Invoke(1);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
                _playerSeen = true;

            // Debug.Log(collision.name);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
                _playerSeen = false;
        }

        private IEnumerator PlumFading(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);

            _plum.SetActive(false);
        }

        private void PrepareFire()
        {
            if (_currentTimeInterval >= _timeInterval)
            {
                FirePlum();
                _spriteRenderer.color = Color.white;
                _currentTimeInterval = 0;
            }
            else
            {
                _spriteRenderer.color -= darkering;
                _currentTimeInterval += Time.deltaTime;
            }
        }

        private void FirePlum()
        {
            _plum.SetActive(true);
            StartCoroutine(PlumFading(_timeInterval / 2));
        }
    }
}
