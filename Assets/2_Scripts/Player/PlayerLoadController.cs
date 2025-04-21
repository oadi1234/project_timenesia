using System.Collections;
using _2_Scripts.Global;
using UnityEngine;

namespace _2_Scripts.Player
{
    public class PlayerLoadController : MonoBehaviour
    {
        [SerializeField] private Transform playerCameraTransform;

        private IEnumerator Start()
        {
            transform.position.Set(GameDataManager.Instance.lastSavePointPosition.x, GameDataManager.Instance.lastSavePointPosition.y, 0);
            if (playerCameraTransform)
                CameraScript.Instance.SetFollow(playerCameraTransform);
            else
            {
                Debug.LogWarning("PlayerCameraTransform is null. Camera now follows base transform of player.");
                CameraScript.Instance.SetFollow(transform);
            }
            CameraScript.Instance.SetInstantSnap(true);
            yield return new WaitForSeconds(0.1f);
            CameraScript.Instance.SetInstantSnap(false);
        }
    }
}
