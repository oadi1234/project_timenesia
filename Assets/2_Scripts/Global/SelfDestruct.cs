using System.Collections;
using UnityEngine;

namespace _2_Scripts.Global
{
    public class SelfDestruct : MonoBehaviour
    {
        [SerializeField] private float destroyAfter = 0;
        
        IEnumerator Start()
        {
            yield return new WaitForSeconds(destroyAfter);
            Destroy(gameObject);
        }
    }
}
