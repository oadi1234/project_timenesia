using System;
using System.Collections;
using _2_Scripts.Enemies.Temp_SecondApproach;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _2_Scripts.Enemies.Temp_Spitter
{
    public class SpitProjectilesState : IState
    {
        private static StaticEnemyBase _enemy;
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
            for (int i = 0; i < 12; i++)
            {
                yield return new WaitForSeconds(0.3f);
                InstantiateProjectile();
            }
            
            yield return new WaitForSeconds(seconds);
            _enemy.SetSightEnabled(true);
            _spitterStateMachine.ChangeState(_spitterStateMachine.HaltState);
        }

        private void InstantiateProjectile()
        {
            Debug.Log("Instant - state");
            (_enemy as IEnemyWithInstantiate)?.InstantiateObject(_spit);
        }
    }
}