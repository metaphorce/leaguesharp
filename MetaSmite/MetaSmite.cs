using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace MetaSmite
{
    internal static class MetaSmite
    {
        internal static Menu Config;
        internal static Obj_AI_Hero Player;
        internal static bool playerSupported = true;

        internal static void Load()
        {
            try
            {
                Game.PrintChat("MetaSmite Loaded!");

                Player = ObjectManager.Player;

                Config = new Menu("MetaSmite", "MetaSmite", true);

                SmiteManager.Load();


                try
                {
                    Type.GetType("MetaSmite.Champions." + Player.ChampionName).GetMethod("Load").Invoke(null, null);
                }
                catch
                {
                    Game.PrintChat(Player.ChampionName + " is not supported. Smite will still work if you have it!");
                }

                Config.AddToMainMenu();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            var rangeDrawStatus = Config.Item("RangeDraw").GetValue<bool>();
            var drawStatus = Config.Item("DrawStatus").GetValue<bool>();
            if (rangeDrawStatus && drawStatus)
            {
                if (Config.Item("Enabled").GetValue<KeyBind>().Active || Config.Item("EnabledH").GetValue<KeyBind>().Active)
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, 570f, System.Drawing.Color.Green);
                }
                else
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, 570f, System.Drawing.Color.Red);
                }
            }
        }
    }
}
