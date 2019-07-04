using Steam.SteamObjects;
using System.Collections.Generic;

namespace RandomizedSteamPick.Tools
{
    public class AlphabeticComparer : IComparer<Game>
    {
        public int Compare(string x, string y) => x.CompareTo(y);

        public int Compare(Game x, Game y)
        {
            return Compare(x.Name, y.Name);
        }
    }
}
