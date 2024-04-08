using UnityEngine;

namespace _2_Scripts.Enemies
{
    public interface IEnemyWithInstantiate
    {
        public void InstantiateObject(GameObject objectToInstantiate, bool inWorldSpace = false);
    }
}