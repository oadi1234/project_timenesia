using System.Collections.Generic;
using System.Linq;
using _2_Scripts.Global.Animation.Model;
using _2_Scripts.Player.model;
using _2_Scripts.UI.Animation.Model;

namespace _2_Scripts.Player.utility
{
    public class EffortArrayComparer : EqualityComparer<List<EffortType>>
    {
        public override bool Equals(List<EffortType> x, List<EffortType> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.SequenceEqual(y);
        }

        public override int GetHashCode(List<EffortType> obj)
        {
            return obj.Select((effortType, i) => (int)effortType * 10 ^ i).Sum();
        }
    }
}