using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enemies
{
    internal interface IEnemy
    {
        public event Action<int> OnEnemyKilled;
    }
}
