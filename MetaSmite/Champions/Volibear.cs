using System;
using LeagueSharp;
using SharpDX;
using LeagueSharp.Common;

namespace MetaSmite.Champions
{
    public class Volibear
    {
        internal static Spell champSpell;
        private static double totalDamage;
        private static double spellDamage;

        public Volibear()
        {
            //Load spells
            champSpell = new Spell(SpellSlot.W, 400f);

            //Spell usage.
            MetaSmite.Config.AddItem(new MenuItem("Enabled-" + MetaSmite.Player.ChampionName, MetaSmite.Player.ChampionName + "-" + champSpell.Slot)).SetValue(true);

            //Events
            Game.OnUpdate += OnGameUpdate;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (MetaSmite.Config.Item("Enabled").GetValue<KeyBind>().Active || MetaSmite.Config.Item("EnabledH").GetValue<KeyBind>().Active)
            {
                if (SmiteManager.mob != null && MetaSmite.Config.Item(SmiteManager.mob.BaseSkinName).GetValue<bool>() && Vector3.Distance(MetaSmite.Player.ServerPosition, SmiteManager.mob.ServerPosition) <= champSpell.Range)
                {
                    spellDamage = MetaSmite.Player.GetSpellDamage(SmiteManager.mob, champSpell.Slot);
                    totalDamage = spellDamage + SmiteManager.damage;
                    if (MetaSmite.Config.Item("Enabled-" + ObjectManager.Player.ChampionName).GetValue<bool>() &&
                        SmiteManager.smite.IsReady() &&
                        champSpell.IsReady() && totalDamage >= SmiteManager.mob.Health)
                    {
                        MetaSmite.Player.Spellbook.CastSpell(champSpell.Slot, SmiteManager.mob);
                    }
                    if (MetaSmite.Config.Item("Enabled-" + ObjectManager.Player.ChampionName).GetValue<bool>() &&
                        champSpell.IsReady() && spellDamage >= SmiteManager.mob.Health)
                    {
                        MetaSmite.Player.Spellbook.CastSpell(champSpell.Slot, SmiteManager.mob);
                    }
                }
            }
        }
    }
}
