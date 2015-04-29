﻿using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace MetaSmite.Champions
{
    public class Amumu
    {
        internal static Spell champSpell;
        private static Menu Config = MetaSmite.Config;
        private static double totalDamage;
        private static double spellDamage;

        public Amumu()
        {
            //Load spells
            champSpell = new Spell(SpellSlot.Q, 1100f);

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
                    spellDamage = MetaSmite.Player.GetSpellDamage(SmiteManager.mob, champSpell.Slot);
                    totalDamage = spellDamage + SmiteManager.damage;
                    var pred = champSpell.GetPrediction(SmiteManager.mob);
                    if (Config.Item("Enabled-" + ObjectManager.Player.ChampionName).GetValue<bool>() &&
                        SmiteManager.smite.IsReady() &&
                        champSpell.IsReady() && totalDamage >= SmiteManager.mob.Health &&
                        pred.Hitchance >= HitChance.Medium)
                    {
                        champSpell.Cast(pred.CastPosition);
                    }
                    if (Config.Item("Enabled-" + ObjectManager.Player.ChampionName).GetValue<bool>() &&
                        champSpell.IsReady() && spellDamage >= SmiteManager.mob.Health &&
                        pred.Hitchance >= HitChance.Medium)
                    {
                        champSpell.Cast(pred.CastPosition);
                    }
                }
            }
        }
    }
}
