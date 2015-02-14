using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace MetaSmite.Champions
{
    public static class Kalista
    {
        internal static Spell champSpell;
        private static Menu Config = MetaSmite.Config;
        private static double totalDamage;
        private static double spellDamage;
        private const string E_BUFF_NAME = "KalistaExpungeMarker";

        //Rend Related data - Credits to Hellsing for all of the Rend Code
        private static float[] rawRendDamage = new float[] { 20, 30, 40, 50, 60 };
        private static float[] rawRendDamageMultiplier = new float[] { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
        private static float[] rawRendDamagePerSpear = new float[] { 10, 14, 19, 25, 32 };
        private static float[] rawRendDamagePerSpearMultiplier = new float[] { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f };

        public static void Load()
        {
            //Load spells
            champSpell = new Spell(SpellSlot.E, 950f);

            //Spell usage.
            Config.AddItem(new MenuItem("Enabled-" + MetaSmite.Player.ChampionName, MetaSmite.Player.ChampionName + "-" + champSpell.Slot)).SetValue(true);
            Config.AddItem(new MenuItem("spellReductionE", "E damage reduction")).SetValue(new Slider(0, 0, 20));

            //Events
            Game.OnGameUpdate += OnGameUpdate;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Config.Item("Enabled").GetValue<KeyBind>().Active || Config.Item("EnabledH").GetValue<KeyBind>().Active)
            {
                if (SmiteManager.mob != null && Config.Item(SmiteManager.mob.BaseSkinName).GetValue<bool>())
                {
                    spellDamage = GetRendDamage(SmiteManager.mob);
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

        public static float GetRendDamage(Obj_AI_Base target, int customStacks = -1)
        {
            // Calculate the damage and return
            return (float)MetaSmite.Player.CalcDamage(target, Damage.DamageType.Physical, GetRawRendDamage(target, customStacks)) - Config.Item("spellReductionE").GetValue<Slider>().Value * 0.98f;
        }

        public static float GetRawRendDamage(Obj_AI_Base target, int customStacks = -1)
        {
            // Get buff
            var buff = target.GetRendBuff();

            if (buff != null || customStacks > -1)
            {
                return (rawRendDamage[champSpell.Level - 1] + rawRendDamageMultiplier[champSpell.Level - 1] * MetaSmite.Player.TotalAttackDamage()) + // Base damage
                       ((customStacks < 0 ? buff.Count : customStacks) - 1) * // Spear count
                       (rawRendDamagePerSpear[champSpell.Level - 1] + rawRendDamagePerSpearMultiplier[champSpell.Level - 1] * MetaSmite.Player.TotalAttackDamage()); // Damage per spear
            }

            return 0;
        }

        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
        {
            return target.Buffs.Find(b => b.DisplayName == E_BUFF_NAME);
        }
    }
}
