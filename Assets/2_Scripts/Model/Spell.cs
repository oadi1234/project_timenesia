using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    internal class Spell
    {
        public EffortElement[] EffortElements;
        public int CostMana => EffortElements.Length;

        public string Name { get; set; }

        public string Description { get; set; }

        public Spell(string name, List<EffortElement> efforts, string description = "")
        {
            Name = name;
            Description = description;
            EffortElements = efforts.ToArray();
        }
    }
}
