using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Serialization;

public class GraphicFlipper : MonoBehaviour
{
    public AIPath aiPath;

    private bool _facingRight = false;
    // Update is called once per frame
    void Update()
    {
        if (aiPath.desiredVelocity.x >= 0.01f && !_facingRight)
        {
            Flip();
            _facingRight = true;
        }

        else if (aiPath.desiredVelocity.x <= -0.01f && _facingRight)
        {
            Flip();
            _facingRight = false;
        }
    }

    private void Flip()
    {
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1f, 1f, 1f));
    }
}
