using System;
using _2___Scripts.Player;

namespace _2___Scripts.Enemies.Attacks
{
    public interface IBaseAttack
    {
        public static event Action<IBaseAttack> OnAttack;
        public string AttackName { get; }
        public Hurt Params { get; }
    }
}
