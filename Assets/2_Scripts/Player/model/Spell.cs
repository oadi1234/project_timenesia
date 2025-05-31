using System.Collections.Generic;
using _2_Scripts.Global.Animation.Model;
using _2_Scripts.UI.Animation.Model;

namespace _2_Scripts.Player.model
{
    internal class Spell
    {
        public EffortType[] EffortElements;
        public int CostMana => EffortElements.Length;

        public string Name { get; set; }

        public string Description { get; set; }

        public Spell(string name, List<EffortType> efforts, string description = "")
        {
            Name = name;
            Description = description;
            EffortElements = efforts.ToArray();
        }
    }
}
