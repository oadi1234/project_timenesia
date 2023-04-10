using System;
using System.Collections;
using System.Collections.Generic;
using _2___Scripts.Global;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    public bool PassThrough = false;
    public void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.gameObject.layer)
        {
            case (int)LayerNames.Enemy:
                Destroy(col.gameObject);
                if (!PassThrough)
                    Destroy(gameObject);
                break;
            case (int)LayerNames.Wall:
                Destroy(gameObject);
                break;
        }
    }
}
