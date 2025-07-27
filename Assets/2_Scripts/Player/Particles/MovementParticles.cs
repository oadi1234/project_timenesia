using _2_Scripts.Player.Controllers;
using _2_Scripts.Player.model;
using UnityEngine;

namespace _2_Scripts.Player.Particles
{
    public class MovementParticles : SpriteFacingDirection
    {
        [SerializeField] private PlayerMovementController playerMovementController;
        [SerializeField] private ParticleSystem stepParticles;
        [SerializeField] private ParticleSystem heavyStompParticles;
        [SerializeField] private float fallingTimeThreshold = 0.1f;


        private ParticleSystem stepParticlesInstance;
        private ParticleSystem heavyStompParticlesInstance;

        private float fallingTime = 0f;
        
        // TODO add raycast checks to see if certain particles should even happen, i.e. if there is no ground below
        //  feet it shouldn't rise up dust.

        private void FixedUpdate()
        {
            CalculateDirection();
            if (playerMovementController.GetYVelocity() < 0f)
                fallingTime += Time.fixedDeltaTime;
            if (fallingTime > fallingTimeThreshold && playerMovementController.IsGrounded())
            {
                SpawnHeavyStompParticles();
            }

            if (playerMovementController.GetYVelocity() > 0f || playerMovementController.IsWallSliding() ||
                playerMovementController.IsGrounded())
                fallingTime = 0f;
        }

        public void SpawnStepDust()
        {
            InstantiateParticles(- 0.207f, - 1.631f, 1f);
        }

        public void SpawnStopDust()
        {
            InstantiateParticles(- 1.243f, - 1.631f, 1f);
        }

        public void SpawnAttackDust1()
        {
            InstantiateParticles(0.721f, - 1.631f, 0.5f);
        }

        public void SpawnAttackFrontDust1()
        {
            InstantiateParticles(- 0.979f, - 1.713f, 0.5f);
        }

        public void SpawnAttackDust2()
        {
            InstantiateParticles(0.602f, - 1.562f, 0.5f);
        }

        public void SpawnAttackFrontDust2()
        {
            InstantiateParticles(- 0.979f, - 1.669f, 0.5f);
        }

        public void SpawnWallslideHandDust()
        {
            InstantiateParticles(-0.95f, 1.83f, 0.5f);
        }

        public void SpawnWallslideLeg1Dust()
        {
            InstantiateParticles(-0.95f, -0.31f, 0.5f);
        }
        public void SpawnWallslideLeg2Dust()
        {
            InstantiateParticles(-0.95f, -1.56f, 1f);
        }

        private void SpawnHeavyStompParticles()
        {
            heavyStompParticlesInstance = Instantiate(heavyStompParticles,
                new Vector3(transform.position.x, transform.position.y - 1.59f, transform.position.z),
                Quaternion.identity);
            var main = heavyStompParticlesInstance.main;
            main.startSizeMultiplier = Mathf.Min(fallingTime / fallingTimeThreshold, 3f);
        }

        private void InstantiateParticles(float xPos, float yPos, float sizeMult)
        {
            var vector = new Vector3(transform.position.x + xPos * Direction, transform.position.y + yPos,
                transform.position.z);
            stepParticlesInstance = Instantiate(stepParticles,
                vector,
                Quaternion.identity);
            if (Mathf.Approximately(sizeMult, 1f)) return;
            var main = stepParticlesInstance.main;
            main.startSizeMultiplier = 0.5f;
        }
    }
}