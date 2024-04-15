using _2_Scripts.Enemies.Attacks;
using _2_Scripts.Player;
using UnityEngine;

namespace _2_Scripts.Global.Hazards
{
    public class DamageOnHit : BaseAttack
    {
        [SerializeField]
        private string attackName = "basic";
        [SerializeField]
        private bool persistent;

        private void Start()
        {
            Params = new Hurt {DamageDealt = 1, IFramesGiven = 2f};
            AttackName = attackName;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == (int) Layers.Player)
            {
                OnAttack(this);
                if (persistent) return;

                gameObject.SetActive(false);
                Destroy(this); //todo: is this 100% safe?
            }
            else if (!persistent && other.gameObject.layer is (int) Layers.Hazard or (int) Layers.Wall)
            {
                gameObject.SetActive(false);
                Destroy(this); //todo: is this 100% safe?
            }
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == (int) Layers.Player)
            {
                OnAttack(this);
                if (persistent) return;
                
                gameObject.SetActive(false);
                Destroy(this); //todo: is this 100% safe?
            }
        }
    }
}