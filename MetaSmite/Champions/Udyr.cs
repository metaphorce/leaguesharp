using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace MetaSmite.Champions
{
    public static class Udyr
    {
        internal static Spell champSpell;
        private static Menu Config = MetaSmite.Config;
        private static double totalDamage;
        private static double spellDamage;

        public static void Load()
        {
            //Load spells
            champSpell = new Spell(SpellSlot.R, ObjectManager.Player.AttackRange);

            //Spell usage.
            Config.AddItem(new MenuItem("Enabled-" + MetaSmite.Player.ChampionName, MetaSmite.Player.ChampionName + "-" + champSpell.Slot)).SetValue(true);

            //Events
            Game.OnGameUpdate += OnGameUpdate;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Config.Item("Enabled").GetValue<KeyBind>().Active || Config.Item("EnabledH").GetValue<KeyBind>().Active)
            {
                if (SmiteManager.mob != null && Config.Item(SmiteManager.mob.BaseSkinName).GetValue<bool>())
                {
                    spellDamage = getUdyrR(SmiteManager.mob);
                    totalDamage = spellDamage + SmiteManager.damage;

                    if (Config.Item("Enabled-" + ObjectManager.Player.ChampionName).GetValue<bool>() &&
                        MetaSmite.Player.Spellbook.CanUseSpell(SmiteManager.smite.Slot) == SpellState.Ready &&
                        champSpell.IsReady() && (totalDamage >= SmiteManager.mob.Health || spellDamage >= SmiteManager.mob.Health))
                    {
                        champSpell.Cast();
                    }
                }
            }
        }

        public static double getUdyrR(Obj_AI_Base target)
        {
            Int32[] dmgQ = { 40, 80, 120, 160, 200 };
            double damage = ObjectManager.Player.CalcDamage(target, Damage.DamageType.Magical, dmgQ[champSpell.Level - 1] + 0.45 * ObjectManager.Player.FlatMagicDamageMod);
            return damage;
        }
    }
}
