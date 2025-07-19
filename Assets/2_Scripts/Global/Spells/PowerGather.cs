using System.Collections;
using _2_Scripts.Player;
using UnityEngine;

namespace _2_Scripts.Global.Spells
{
    public class PowerGather : MonoBehaviour
    {
        //TODO: This class is mostly for instantiating an animation for "gathering magic power" animations and
        // their interruption.
        // Might be an idea to rework buff spells to handle this as well.
        [SerializeField] private GameObject nextSpellStage;
        [SerializeField] private float spawnNextStageAfter;
        [SerializeField] private float aliveTimeAfterSpawn;

        private GameObject nextStageInstance;

        private Quaternion targetRotation;
        private Transform parentObject;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(spawnNextStageAfter);
            DetachFromParentObject();
            nextStageInstance = Instantiate(nextSpellStage, parentObject.parent);
            nextStageInstance.transform.rotation = targetRotation;
            nextStageInstance.transform.localScale = transform.localScale;
            yield return new WaitForSeconds(aliveTimeAfterSpawn);
            Destroy(gameObject);
        }

        private void DetachFromParentObject()
        {
            targetRotation = transform.parent.rotation;
            parentObject = transform.parent;
            transform.parent = null;
            transform.rotation = targetRotation;
        }

    }
}