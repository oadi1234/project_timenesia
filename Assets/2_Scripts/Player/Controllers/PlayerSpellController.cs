using _2_Scripts.Global;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class PlayerSpellController : MonoBehaviour
    {
        public bool PassThrough = false;
        public void OnTriggerEnter2D(Collider2D col)
        {
            switch (col.gameObject.layer)
            {
                case (int)Layers.Enemy:
                    Destroy(col.gameObject);
                    if (!PassThrough)
                        Destroy(gameObject);
                    break;
                case (int)Layers.Wall:
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
