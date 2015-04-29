using System;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using System.Reflection;
using System.Reflection.Emit;

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
                    Invoker.Invoke("MetaSmite.Champions." + Player.ChampionName);
                }
                catch(Exception e)
                {
                    Game.PrintChat(Player.ChampionName + " is not supported. Smite will still work if you have it!");
                    Console.WriteLine(e);
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

    public static class Invoker
    {
        public static void Invoke(string typeName)
        {
            Type type = Type.GetType(typeName);
            NewInstance(type);
        }

        private static void NewInstance(Type type)
        {
            var target = type.GetConstructor(Type.EmptyTypes);
            var dynamic = new DynamicMethod(string.Empty, type, new Type[0], target.DeclaringType);
            var il = dynamic.GetILGenerator();
            il.DeclareLocal(target.DeclaringType);
            il.Emit(OpCodes.Newobj, target);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            ((Func<object>)dynamic.CreateDelegate(typeof(Func<object>)))();
        }
    }
}
