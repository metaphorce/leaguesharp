using System;
using LeagueSharp.Common;

namespace MetaSmite
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            MetaSmite.Load();
        }
    }
}
