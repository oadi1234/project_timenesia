using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enemies.Attacks
{
    public interface IBaseAttack
    {
        public static event Action<IBaseAttack> OnAttack;
        public string AttackName { get; }
    }
}
