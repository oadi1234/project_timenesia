using UnityEngine;

namespace _2_Scripts.Global.Spells.utility
{
    public class BlastRandomizer : MonoBehaviour
    {
        [SerializeField] private bool randomRotation;
        [SerializeField] private float maxAngleDeviation = 15f;

        [SerializeField] private bool randomScale;
        [SerializeField] private float maxScaleMultiplier = 1.1f;

        [SerializeField] private bool randomXFlip;
        [SerializeField] private bool randomYFlip;
        // Start is called before the first frame update
        void Start()
        {
            if (randomRotation)
                transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-maxAngleDeviation, maxAngleDeviation));
            if (randomScale)
            {
                var scale = Random.Range(2 - maxScaleMultiplier, maxScaleMultiplier);
                transform.localScale = new Vector3(1 * scale, 1 * scale, 1);
            }
            if (randomXFlip)
            {
                int flipX = (Random.Range(0, 2) << 1) - 1;
                transform.localScale = new Vector3(1 * flipX , 1 , 1);
            }
            if (randomYFlip)
            {
                int flipY = (Random.Range(0, 2) << 1) - 1;
                transform.localScale = new Vector3(1 , 1 * flipY , 1);
            }
        }
    }
}
