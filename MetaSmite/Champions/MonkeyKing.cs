﻿using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace MetaSmite.Champions
{
    public class MonkeyKing
    {
        internal static Spell champSpell;
        private static Menu Config = MetaSmite.Config;
        private static double totalDamage;
        private static double spellDamage;

        public MonkeyKing()
        {
            //Load spells
            champSpell = new Spell(SpellSlot.Q, 100f);

            //Spell usage.
            Config.AddItem(new MenuItem("Enabled-" + MetaSmite.Player.ChampionName, MetaSmite.Player.ChampionName + "-" + champSpell.Slot)).SetValue(true);

            //Events
            Game.OnUpdate += OnGameUpdate;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Config.Item("Enabled").GetValue<KeyBind>().Active || Config.Item("EnabledH").GetValue<KeyBind>().Active)
            {
                if (SmiteManager.mob != null && Config.Item(SmiteManager.mob.BaseSkinName).GetValue<bool>())
                {
                    spellDamage = MetaSmite.Player.GetSpellDamage(SmiteManager.mob, champSpell.Slot);
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
    }
}
