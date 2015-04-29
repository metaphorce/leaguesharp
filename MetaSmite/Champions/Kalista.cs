using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace MetaSmite.Champions
{
    public class Kalista
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

        public Kalista()
        {
            //Load spells
            champSpell = new Spell(SpellSlot.E, 950f);

            //Spell usage.
            Config.AddItem(new MenuItem("Enabled-" + MetaSmite.Player.ChampionName, MetaSmite.Player.ChampionName + "-" + champSpell.Slot)).SetValue(true);
            Config.AddItem(new MenuItem("spellReductionE", "E damage reduction")).SetValue(new Slider(0, 0, 20));

            //Events
            Game.OnUpdate += OnGameUpdate;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Config.Item("Enabled").GetValue<KeyBind>().Active || Config.Item("EnabledH").GetValue<KeyBind>().Active)
            {
                if (SmiteManager.mob != null && Config.Item(SmiteManager.mob.BaseSkinName).GetValue<bool>() && Vector3.Distance(MetaSmite.Player.ServerPosition, SmiteManager.mob.ServerPosition) <= champSpell.Range)
                {
                    spellDamage = GetRendDamage(SmiteManager.mob);
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

        // ALL REND DAMAGE CODE IS WRITTEN BY dongu54321.
        public static float GetRendDamage(Obj_AI_Base target)
        {
            var buff = target.Buffs.Find(b => b.Caster.IsMe && b.IsValidBuff() && b.DisplayName == "KalistaExpungeMarker"); ;
            if (buff != null && champSpell.IsReady())
            {
                var a = Config.Item("spellReductionE").GetValue<Slider>().Value;
                double armorPenPercent = ObjectManager.Player.PercentArmorPenetrationMod;
                double armorPenFlat = ObjectManager.Player.FlatArmorPenetrationMod;
                double k;
                double damage = 0f;
                var armor = target.Armor;
                if (armor < 0) { k = 2 - 100 / (100 - armor); }
                else if ((target.Armor * armorPenPercent) - armorPenFlat < 0) k = 1;
                else { k = 100 / (100 + (target.Armor * armorPenPercent) - armorPenFlat); }
                if (ObjectManager.Player.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1)) k = k * 1.015;
                if (ObjectManager.Player.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 146 && m.Points == 1)) k = k * 1.03;
                damage += new double[] { 20, 30, 40, 50, 60 }[champSpell.Level - 1] + ObjectManager.Player.TotalAttackDamage * 0.6f + (new double[] { 10, 14, 19, 25, 32 }[champSpell.Level - 1] + new double[] { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f }[champSpell.Level - 1] * ObjectManager.Player.TotalAttackDamage) * (buff.Count - 1);
                return (float)(damage * k - a);
            }
            else
            {
                return -1;
            }
        }
    }
}
