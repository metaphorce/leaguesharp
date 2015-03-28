using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace MetaSmite.Champions
{
    public static class LeeSin
    {
        internal static Spell champSpell;
        private static Menu Config = MetaSmite.Config;
        private static double totalDamage;
        private static double spellDamage;

        public static void Load()
        {
            //Load spells
            champSpell = new Spell(SpellSlot.Q, 1300f);

            //Spell usage.
            Config.AddItem(new MenuItem("Enabled-" + MetaSmite.Player.ChampionName, MetaSmite.Player.ChampionName + "-" + champSpell.Slot)).SetValue(true);

            //Events
            Game.OnUpdate += OnGameUpdate;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Config.Item("Enabled").GetValue<KeyBind>().Active || Config.Item("EnabledH").GetValue<KeyBind>().Active)
            {
                if (SmiteManager.mob != null && Config.Item(SmiteManager.mob.BaseSkinName).GetValue<bool>() && Vector3.Distance(MetaSmite.Player.ServerPosition, SmiteManager.mob.ServerPosition) <= champSpell.Range)
                {
                    spellDamage = getQ2Dmg(SmiteManager.mob);
                    totalDamage = spellDamage + SmiteManager.damage;
                    if (Config.Item("Enabled-" + ObjectManager.Player.ChampionName).GetValue<bool>() &&
                        SmiteManager.smite.IsReady() &&
                        champSpell.IsReady() && totalDamage >= SmiteManager.mob.Health)
                    {
                        champSpell.Cast();
                    }
                    if (Config.Item("Enabled-" + ObjectManager.Player.ChampionName).GetValue<bool>() &&
                        champSpell.IsReady() && spellDamage >= SmiteManager.mob.Health)
                    {
                        champSpell.Cast();
                    }
                }
            }
        }

        public static double getQ2Dmg(Obj_AI_Base target)
        {
            Int32[] dmgQ = { 50, 80, 110, 140, 170 };
            double damage = ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, dmgQ[champSpell.Level - 1] + 0.9 * ObjectManager.Player.FlatPhysicalDamageMod + 0.08 * (target.MaxHealth - target.Health));
            if (damage > 400)
            {
                return 400;
            }
            return damage;
        }
    }
}
