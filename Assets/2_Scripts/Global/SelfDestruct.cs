using System;
using System.Collections;
using UnityEngine;

namespace _2_Scripts.Global
{
    public class SelfDestruct : MonoBehaviour
    {
        [SerializeField] private float destroyAfter = 0;
        
        public event Action OnSelfDestruct;
        
        IEnumerator Start()
        {
            yield return new WaitForSeconds(destroyAfter);
            OnSelfDestruct?.Invoke();
            Destroy(gameObject);
        }
    }
}
