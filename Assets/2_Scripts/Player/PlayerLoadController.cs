using _2___Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoadController : MonoBehaviour
{

    private void Start()
    {
        var transform = GetComponent<Transform>();
        transform.position.Set(GameDataManager.Instance.LastSavePointPosition.x, GameDataManager.Instance.LastSavePointPosition.y, 0);
        CameraScript.Instance.SetFollow(transform);
    }
}
