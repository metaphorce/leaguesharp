using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;
using SharpDX;

namespace MetaSmite
{
    public static class SmiteManager
    {
        public static bool hasSmite;
        private static Menu Config = MetaSmite.Config;
        private static SpellDataInst Summoner1;
        private static SpellDataInst Summoner2;
        public static Spell smite;
        public static double damage;
        private static int Plevel;
        public static Obj_AI_Base mob;
        private static string[] MinionNames = 
        {
            "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith",
            "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", 
            "SRU_Red", "SRU_Krug", "SRU_Dragon", "Sru_Crab", "SRU_Baron"
        };



        static SmiteManager()
        {
            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
        }

        internal static void Load()
        {
            //Menu Setup
            Config.AddItem(new MenuItem("Enabled", "Toggle Enabled").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle, true)));
            Config.AddItem(new MenuItem("EnabledH", "Hold Enable").SetValue(new KeyBind("K".ToCharArray()[0], KeyBindType.Press)));
            Config.AddItem(new MenuItem("RangeDraw", "Draw Range and Status").SetValue(true));
            Config.AddItem(new MenuItem("DrawStatus", "Drawings off/on!").SetValue(true));
            setupCampMenu();


            Summoner1 = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner1);
            Summoner2 = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner2);
            int level = ObjectManager.Player.Level;
            Plevel = level;

            if (new[] { "s5_summonersmiteplayerganker", "itemsmiteaoe", "s5_summonersmitequick", "s5_summonersmiteduel", "summonersmite" }.Contains(Summoner1.Name))
            {
                smite = new Spell(SpellSlot.Summoner1, 570f);
                setSmiteDamage();
            }

            if (new[] { "s5summonersmiteplayerganker", "itemsmiteaoe", "s5_summonersmitequick", "s5_summonersmiteduel", "summonersmite" }.Contains(Summoner2.Name))
            {
                smite = new Spell(SpellSlot.Summoner2, 570f);
                setSmiteDamage();
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if(ObjectManager.Player.Level > Plevel)
            {
                Plevel = ObjectManager.Player.Level;
                setSmiteDamage();
            }
            if (Config.Item("Enabled").GetValue<KeyBind>().Active || Config.Item("EnabledH").GetValue<KeyBind>().Active)
            {
                mob = GetNearest(ObjectManager.Player.ServerPosition);
                //Game.PrintChat("Mobname is: " + mob.Name + " range is: " + smite.Range);
                if (mob != null && Config.Item(mob.BaseSkinName).GetValue<bool>())
                {
                    if (MetaSmite.Player.Spellbook.CanUseSpell(smite.Slot) == SpellState.Ready && damage >= mob.Health && Vector3.Distance(ObjectManager.Player.ServerPosition, mob.ServerPosition) <= smite.Range)
                    {
                        MetaSmite.Player.Spellbook.CastSpell(smite.Slot, mob);
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (mob != null && Config.Item(mob.BaseSkinName).GetValue<bool>())
            {
                //Drawing will eventually go back here
            }
        }

        public static void setSmiteDamage()
        {
            int level = ObjectManager.Player.Level;
            int[] smitedamage =
            {
                20*level + 370,
                30*level + 330,
                40*level + 240,
                50*level + 100
            };
            damage = smitedamage.Max();
        }

        public static Obj_AI_Minion GetNearest(Vector3 pos)
        {
            var minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(minion => minion.IsValid && MinionNames.Any(name => minion.Name.StartsWith(name)) && !MinionNames.Any(name => minion.Name.Contains("Mini")) && !MinionNames.Any(name => minion.Name.Contains("Spawn")));
            var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
            Obj_AI_Minion sMinion = objAiMinions.FirstOrDefault();
            double? nearest = null;
            foreach (Obj_AI_Minion minion in objAiMinions)
            {
                double distance = Vector3.Distance(pos, minion.Position);
                if (nearest == null || nearest > distance)
                {
                    nearest = distance;
                    sMinion = minion;
                }
            }
            return sMinion;
        }

        public static void setupCampMenu()
        {
            Config.AddSubMenu(new Menu("Camps", "Camps"));
            if (Game.MapId == GameMapId.TwistedTreeline)
            {
                Config.SubMenu("Camps").AddItem(new MenuItem("TT_Spiderboss", "Vilemaw Enabled").SetValue(true));
                Config.SubMenu("Camps").AddItem(new MenuItem("TT_NGolem", "Golem Enabled").SetValue(true));
                Config.SubMenu("Camps").AddItem(new MenuItem("TT_NWolf", "Wolf Enabled").SetValue(true));
                Config.SubMenu("Camps").AddItem(new MenuItem("TT_NWraith", "Wraith Enabled").SetValue(true));
            }
            if (Game.MapId == (GameMapId)11)
            {
                Config.SubMenu("Camps").AddItem(new MenuItem("SRU_Baron", "Baron Enabled").SetValue(true));
                Config.SubMenu("Camps").AddItem(new MenuItem("SRU_Dragon", "Dragon Enabled").SetValue(true));
                Config.SubMenu("Camps").AddItem(new MenuItem("SRU_Blue", "Blue Enabled").SetValue(true));
                Config.SubMenu("Camps").AddItem(new MenuItem("SRU_Red", "Red Enabled").SetValue(true));
                Config.SubMenu("Camps").AddItem(new MenuItem("SRU_Gromp", "Gromp Enabled").SetValue(false));
                Config.SubMenu("Camps").AddItem(new MenuItem("SRU_Murkwolf", "Murkwolf Enabled").SetValue(false));
                Config.SubMenu("Camps").AddItem(new MenuItem("SRU_Krug", "Krug Enabled").SetValue(false));
                Config.SubMenu("Camps").AddItem(new MenuItem("SRU_Razorbeak", "Razorbeak Enabled").SetValue(false));
                Config.SubMenu("Camps").AddItem(new MenuItem("Sru_Crab", "Crab Enabled").SetValue(false));
            }
        }

    }
}
