using System.Collections;
using _2_Scripts.Player;
using _2_Scripts.Player.Controllers;
using UnityEngine;

namespace _2_Scripts.Global.Spells
{
    public class PowerGatherHandler : AbstractInterruptibleSpell
    {
        [SerializeField] private GameObject nextSpellStage;
        [SerializeField] private float spawnNextStageAfter;
        [SerializeField] private float aliveTimeAfterSpawn;

        private GameObject nextStageInstance;

        private SelfDestruct selfDestruct;
        private PlayerMovementController playerMovementController;
        private PlayerInputManager playerInputManager;
        private Vector3 direction;

        protected override void Awake()
        {
            selfDestruct = GetComponent<SelfDestruct>();
            playerMovementController = transform.GetComponentInParent<PlayerMovementController>();
            playerInputManager = transform.GetComponentInParent<PlayerInputManager>();
            base.Awake();
        }

        private void FixedUpdate()
        {
            direction = Quaternion.Euler(0, 0, playerInputManager.GetAngle()) * Vector3.right;
            if (transform.localScale.x < 0)
            {
                direction.y *= -1;
            }
            transform.rotation = Quaternion.FromToRotation(direction, Vector3.right);
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(spawnNextStageAfter);
            InstantiateNextStage();
            DetachFromParent();
            DeregisterEvents();
            yield return new WaitForSeconds(aliveTimeAfterSpawn);
            Destroy(gameObject);
        }
        
        protected override void DeregisterEvents()
        {
            base.DeregisterEvents();
            if (selfDestruct)
                selfDestruct.OnSelfDestruct -= DeregisterEvents;
            if (playerMovementController)
                playerMovementController.Flipped -= FlipObject;
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();
            if (selfDestruct)
                selfDestruct.OnSelfDestruct += DeregisterEvents;
            if (playerMovementController)
                playerMovementController.Flipped += FlipObject;
        }

        private void FlipObject(bool isFacingleft)
        {
            // since this animation does not have any state that blocks flipping sprites, it should be safe to use PMC
            transform.localScale = new Vector3(isFacingleft? 1 : -1, transform.localScale.y, transform.localScale.z);
        }

        private void InstantiateNextStage()
        {
            nextStageInstance = Instantiate(nextSpellStage, transform);
            nextStageInstance.transform.rotation = transform.rotation;
            nextStageInstance.transform.parent = transform.parent;
            nextStageInstance.transform.localScale = new Vector3(transform.localScale.x, 1, 1);
        }

        private void DetachFromParent()
        {
            transform.parent = null;
        }

    }
}