using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Reflection;
using SharpDX;

namespace meta_Smite
{
    class Program
    {
        public static Menu Config;
        public static Dictionary<string, SpellSlot> spellList = new Dictionary<string, SpellSlot>();
        public static Dictionary<string, float> rangeList = new Dictionary<string, float>();
        public static SpellSlot smiteSlot = SpellSlot.Unknown;
        public static Spell smite;
        public static Spell champSpell;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameStart;
        }

        private static void Game_OnGameStart(EventArgs args)
        {
            Game.PrintChat("Starting load of Meta Smite");
            setSmiteSlot();
            if(smiteSlot == SpellSlot.Unknown)
            {
                Game.PrintChat("Smite not found, disabling Meta Smite");
                return;
            }
            Config = new Menu("metaSmite", "metaSmite", true);
            Config.AddItem(new MenuItem("Enabled", "Enabled").SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle, true)));
            champSpell = addSupportedChampSkill();
            Config.AddToMainMenu();
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.PrintChat("Meta Smite by metaphorce Loaded");
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Config.Item("Enabled").GetValue<KeyBind>().Active)
            {
                Obj_AI_Base mob = GetNearest(ObjectManager.Player.ServerPosition);
                if (mob != null)
                {
                    double smitedamage = smiteDamage();
                    bool smiteReady = false;
                    bool spellReady = false;
                    if (ObjectManager.Player.SummonerSpellbook.CanUseSpell(smiteSlot) == SpellState.Ready && Vector3.Distance(ObjectManager.Player.ServerPosition, mob.ServerPosition) < smite.Range)
                    {
                        smiteReady = true;
                    }
                    if (Config.Item("Enabled-" + ObjectManager.Player.ChampionName).GetValue<bool>())
                    {
                        spellReady = true;
                    }

                    if (smiteReady && mob.Health < smitedamage) //Smite is ready and enemy is killable with smite
                    {
                        Game.PrintChat("Heath is below needed just smite");
                        ObjectManager.Player.SummonerSpellbook.CastSpell(smiteSlot, mob);
                    }

                    if (champSpell.IsReady() && spellReady && Vector3.Distance(ObjectManager.Player.ServerPosition, mob.ServerPosition) < champSpell.Range + mob.BoundingRadius) //skill is ready 
                    {
                        if (smiteReady)
                        {
                            if (mob.Health < smitedamage + ObjectManager.Player.GetSpellDamage(mob, champSpell.Slot)) //Smite is ready and combined damage will kill
                            {
                                Game.PrintChat("Killing With Combo");
                                ObjectManager.Player.Spellbook.CastSpell(champSpell.Slot, mob);
                            }
                            else if (mob.Health < ObjectManager.Player.GetSpellDamage(mob, champSpell.Slot)) //Killable with spell
                            {
                                Game.PrintChat("Killing With Spell");
                                ObjectManager.Player.Spellbook.CastSpell(champSpell.Slot, mob);
                            }
                        }
                    }
                }
            }
        }

        public static void setSmiteSlot()
        {
            var spells = ObjectManager.Player.SummonerSpellbook.Spells;
            foreach (var spell in spells.Where(spell => spell.Name.ToLower() == "summonersmite"))
            {
                smiteSlot = spell.Slot;
                smite = new Spell(smiteSlot, 700);
                return;
            }
        }

        //public static double adjustDamage(double damage)
        //{
        //    double result = damage;
        //    if (Items.HasItem(1080)) //Spirit Stone
        //    {
        //        result = damage + (damage * 0.2);
        //    }
        //    if (Items.HasItem(3209)) //Spirit of the Elder Lizard
        //    {
        //        result = damage + (damage * 0.2);
        //    }
        //    if (Items.HasItem(3206)) //Spirit of the Spectral Wraith
        //    {
        //        result = damage + (damage * 0.3);
        //    }
        //    return result;
        //}

        public static double smiteDamage()
        {
            int level = ObjectManager.Player.Level;
            int[] damage =
            {
                20*level + 370,
                30*level + 330,
                40*level + 240,
                50*level + 100
            };
            return damage.Max();
        }

        public static Spell addSupportedChampSkill()
        {
            spellList.Add("Chogath", SpellSlot.R);
            spellList.Add("Nunu", SpellSlot.Q);
            spellList.Add("Elise", SpellSlot.Q);
            spellList.Add("Kayle", SpellSlot.Q);
            spellList.Add("Lux", SpellSlot.R);
            spellList.Add("Volibear", SpellSlot.W);
            spellList.Add("Warwick", SpellSlot.Q);
            spellList.Add("Olaf", SpellSlot.E);
            spellList.Add("Twitch", SpellSlot.E);
            spellList.Add("Shaco", SpellSlot.E);
            spellList.Add("Vi", SpellSlot.E);
            spellList.Add("Pantheon", SpellSlot.Q);
            spellList.Add("MasterYi", SpellSlot.Q);
            spellList.Add("MonkeyKing", SpellSlot.Q);
            spellList.Add("Khazix", SpellSlot.Q);
            spellList.Add("Rammus", SpellSlot.Q);

            if(spellList.ContainsKey(ObjectManager.Player.ChampionName))
            {
                string champ = ObjectManager.Player.ChampionName;
                SpellSlot slot;
                spellList.TryGetValue(champ, out slot);
                Spell comboSpell = new Spell(slot, 0);
                comboSpell.Range = getRange(champ);
                Config.AddItem(new MenuItem("Enabled-" + champ, "Enabled-" + champ + "-" + slot)).SetValue(true);
                return comboSpell;
            }
            else
            {
                return new Spell(SpellSlot.Unknown, 0);
            }
        }

        public static float getRange(string champName)
        {
            rangeList.Add("Chogath", 175f);
            rangeList.Add("Nunu", 145f);
            rangeList.Add("Elise", 475f);
            rangeList.Add("Kayle", 650f);
            rangeList.Add("Lux", 3340f);
            rangeList.Add("Volibear", 400f);
            rangeList.Add("Warwick", 400f);
            rangeList.Add("Olaf", 325f);
            rangeList.Add("Twitch", 1200);
            rangeList.Add("Shaco", 625f);
            rangeList.Add("Vi", 600f);
            rangeList.Add("Pantheon", 600f);
            rangeList.Add("MasterYi", 600f);
            rangeList.Add("MonkeyKing", 100f);
            rangeList.Add("Khazix", 325f);
            rangeList.Add("Rammus", 100f);
            float res;
            rangeList.TryGetValue(champName, out res);
            return res;
        }

        //Credits to Lizzaran
        private static readonly string[] MinionNames =
        {
            "Worm", "Dragon", "LizardElder", "AncientGolem", "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith"
        };

        public static Obj_AI_Minion GetNearest(Vector3 pos)
        {
            var minions =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(minion => minion.IsValid && MinionNames.Any(name => minion.Name.StartsWith(name)));
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
    }
}
