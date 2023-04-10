using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    [SerializeField]
    private Transform player;

    private float smooth = 15f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(player.position.x, player.position.y, -10);

        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, player.position.x, Time.deltaTime * smooth),
            Mathf.Lerp(transform.position.y, player.position.y, Time.deltaTime * smooth),
            -10);
    }
}
