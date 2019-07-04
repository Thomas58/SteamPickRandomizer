using Steam.SteamObjects;
using System.Collections.Generic;

namespace RandomizedSteamPick.Model
{
    public class SteamList
    {
        public string Username = "";
        public ulong SteamID = 0;
        public List<Game> GameList = new List<Game>();
        public List<Game> IgnoreList = new List<Game>();

        public SteamList() { }
        public SteamList(List<Game> list)
        {
            this.GameList = list;
        }
    }
}
