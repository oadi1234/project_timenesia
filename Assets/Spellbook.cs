using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellbook : MonoBehaviour
{
    public GameObject FireBall;
    public GameObject EyeBall;
    
    private GameObject _currentSpell;

    public void CastFireBall(int facingRightMultiplier = 1)
    {
        var spell = Instantiate(FireBall, this.gameObject.transform, false);
        spell.transform.parent = null;
        spell.GetComponent<Rigidbody2D>().velocity = new Vector2(facingRightMultiplier* 10, 0);
    }

    public void CastEyeBall(int facingRightMultiplier = 1)
    {
        var spell = Instantiate(EyeBall, this.gameObject.transform, false);
        var spell1 = Instantiate(EyeBall, this.gameObject.transform, false);
        var spell2 = Instantiate(EyeBall, this.gameObject.transform, false);
        spell.transform.parent = null;
        spell1.transform.parent = null;
        spell2.transform.parent = null;
        spell1.transform.rotation = Quaternion.Euler(0, 180 ,-15 );
        spell2.transform.rotation = Quaternion.Euler( 0, 180 ,15 );
        spell.GetComponent<Rigidbody2D>().velocity = new Vector2(facingRightMultiplier* 7, 0);
        spell1.GetComponent<Rigidbody2D>().velocity = new Vector2(facingRightMultiplier* 7, 1);
        spell2.GetComponent<Rigidbody2D>().velocity = new Vector2(facingRightMultiplier* 7, -1);
    }
}
