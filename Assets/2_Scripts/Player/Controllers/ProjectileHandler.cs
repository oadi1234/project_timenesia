using _2_Scripts.Global;
using UnityEngine;

//TODO it does not seem to be player-only. Change namespace.
//Renamed from PlayerSpellController to ProjectileHandler to better show what it does.
namespace _2_Scripts.Player.Controllers
{
    public class ProjectileHandler : MonoBehaviour
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
