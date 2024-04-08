using System.Collections;
using _2_Scripts.Enemies.FSM;
using UnityEngine;

namespace _2_Scripts.Enemies.States
{
    public class SpitProjectilesState : IState
    {
        private readonly StaticEnemyBase _enemy;
        private readonly SpitterStateMachine _spitterStateMachine;
        private readonly GameObject _spit;
        // public static event Action OnSpitting;

        public SpitProjectilesState(StaticEnemyBase enemy, GameObject theSpit, SpitterStateMachine spitterStateMachine)
        {
            _enemy = enemy;
            _spitterStateMachine = spitterStateMachine;
            _spit = theSpit;
        }

        public void OnEnter()
        {
            _enemy.SpriteRenderer.color = Color.red;
            OnLogic();
        }

        public void OnExit()
        {
            _enemy.SpriteRenderer.color = Color.white;
        }

        public void OnLogic()
        {
            _enemy.StartCoroutine(Spitting(5f));
        }
        
        private IEnumerator Spitting(float seconds)
        {
            for (int i = 0; i < 15; i++)
            {
                yield return new WaitForSeconds(0.4f);
                InstantiateProjectile();
            }
            
            yield return new WaitForSeconds(seconds);
            _enemy.SetSightEnabled(true);
            _spitterStateMachine.ChangeState(_spitterStateMachine.HaltState);
        }

        private void InstantiateProjectile()
        {
            (_enemy as IEnemyWithInstantiate)?.InstantiateObject(_spit);
        }
    }
}