namespace _2_Scripts.Global.Health
{
    public interface IDamageable
    {
        void OnHit(float damage);
        void OnDeath();
    }
}