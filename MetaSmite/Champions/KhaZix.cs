using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;
using SharpDX;

namespace MetaSmite.Champions
{
    public static class KhaZix
    {
        internal static Spell champSpell;
        private static Menu Config = MetaSmite.Config;
        private static double totalDamage;
        private static double spellDamage;

        public static void Load()
        {
            //Load spells
            champSpell = new Spell(SpellSlot.Q, 325f);

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
                    spellDamage = getKhazixDmg(SmiteManager.mob);
                    totalDamage = spellDamage + SmiteManager.damage;
                    if (Config.Item("Enabled-" + ObjectManager.Player.ChampionName).GetValue<bool>() &&
                        SmiteManager.smite.IsReady() &&
                        champSpell.IsReady() && totalDamage >= SmiteManager.mob.Health)
                    {
                        MetaSmite.Player.Spellbook.CastSpell(champSpell.Slot, SmiteManager.mob);
                    }
                    if (Config.Item("Enabled-" + ObjectManager.Player.ChampionName).GetValue<bool>() &&
                        champSpell.IsReady() && spellDamage >= SmiteManager.mob.Health)
                    {
                        MetaSmite.Player.Spellbook.CastSpell(champSpell.Slot, SmiteManager.mob);
                    }
                }
            }
        }

        public static double getKhazixDmg(Obj_AI_Base target)
        {
            List<Obj_AI_Base> allMobs = MinionManager.GetMinions(target.ServerPosition, 500f, MinionTypes.All, MinionTeam.Neutral);
            Int32[] dmgQ = { 70, 95, 120, 145, 170 };
            double damage = ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, (dmgQ[champSpell.Level - 1] + (1.2 * ObjectManager.Player.FlatPhysicalDamageMod)));
            if (allMobs.Count == 1)
            {
                if (ObjectManager.Player.HasBuff("khazixqevo", true))
                {
                    return (damage * 1.3) + (ObjectManager.Player.Level * 10) + (1.04 * ObjectManager.Player.FlatPhysicalDamageMod);
                }
                return damage + (damage * 0.3);
            }
            return damage;
        }
    }
}
