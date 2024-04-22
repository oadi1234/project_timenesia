using _2_Scripts.Global;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerLoadController : MonoBehaviour
    {

        private void Start()
        {
            var transform = GetComponent<Transform>();
            transform.position.Set(GameDataManager.Instance.lastSavePointPosition.x, GameDataManager.Instance.lastSavePointPosition.y, 0);
            CameraScript.Instance.SetFollow(transform);
        }
    }
}
