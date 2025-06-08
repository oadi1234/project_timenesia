using System;
using System.Collections.Generic;
using _2_Scripts.UI.Elements.Menu;

namespace _2_Scripts.Global.Utility
{
    public class SpellUISeenComparer : IComparer<UISpell>
    {
        public int Compare(UISpell x, UISpell y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            int compareResult = Convert.ToInt32(x.Seen()) - Convert.ToInt32(y.Seen());
            if (compareResult == 0) return new SpellUINameComparer().Compare(x, y);
            return compareResult;
        }
    }
}