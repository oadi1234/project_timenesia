using System.Collections.Generic;
using _2_Scripts.UI.Elements.Menu;

namespace _2_Scripts.Global.Utility
{
    public class SpellUIEffortLengthComparer : IComparer<UISpell>
    {
        public int Compare(UISpell x, UISpell y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            var lengthCompare = x.GetEffortCombinationLength() - y.GetEffortCombinationLength();
            return lengthCompare == 0 ? new SpellUINameComparer().Compare(x, y) : lengthCompare;
        }
    }
}