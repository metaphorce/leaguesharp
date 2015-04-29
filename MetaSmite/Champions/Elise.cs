﻿using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace MetaSmite.Champions
{
    public class Elise
    {
        internal static Spell champSpell;
        private static Menu Config = MetaSmite.Config;
        private static double totalDamage;
        private static double spellDamage;

        public Elise()
        {
            //Load spells
            champSpell = new Spell(SpellSlot.Q, 475f);

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
    }
}
