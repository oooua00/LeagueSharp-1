using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Cassiopeia
{
    internal class Configs
    {
        public static Menu CassioConfig;
        public static Menu TargetSelectorMenu;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Init()
        {
            CassioConfig = new Menu("Princess " + ObjectManager.Player.ChampionName, "Princess" + ObjectManager.Player.ChampionName, true);

            //Orbwalker
            CassioConfig.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(CassioConfig.SubMenu("Orbwalking"));

            //Targetselector
            TargetSelectorMenu = new Menu("Target Selector", "Common_TargetSelector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            CassioConfig.AddSubMenu(TargetSelectorMenu);

            //Manamanager
            var manaManager = new Menu("Manamanager", "apollo.cassio.manamanager");
            {
                var harass = new Menu("Harass", "apollo.cassio.manamanager.harass");
                {
                    harass.AddItem(
                        new MenuItem("apollo.cassio.manamanager.harass.mana", "Min Mana").SetValue(new Slider(30)));
                    manaManager.AddSubMenu(harass);
                }
                var laneClear = new Menu("Laneclear", "apollo.cassio.manamanager.laneclear");
                {
                    laneClear.AddItem(
                        new MenuItem("apollo.cassio.manamanager.laneclear.mana", "Min Mana").SetValue(new Slider(30)));
                    manaManager.AddSubMenu(laneClear);
                }
                CassioConfig.AddSubMenu(manaManager);
            }
            //Spells
            var spellQ = new Menu("Q", "apollo.cassio.q");
            {
                spellQ.AddSubMenu(new Menu("Modes", "apollo.cassio.q.modes"));
                spellQ.SubMenu("apollo.cassio.q.modes").AddItem(new MenuItem("apollo.cassio.q.modes.combo", "Use in Combo").SetValue(true));
                spellQ.SubMenu("apollo.cassio.q.modes").AddItem(new MenuItem("apollo.cassio.q.modes.harass", "Use in Harass").SetValue(true));
                spellQ.SubMenu("apollo.cassio.q.modes").AddItem(new MenuItem("apollo.cassio.q.modes.laneclear", "Use in LaneClear").SetValue(true));

                var laneClear = new Menu("Laneclear", "apollo.cassio.q.laneclear");
                {
                    laneClear.AddItem(
                        new MenuItem("apollo.cassio.q.laneclear.poi", "Only use on non Poisoned CS").SetValue(false));
                    spellQ.AddSubMenu(laneClear);
                }

                spellQ.AddItem(new MenuItem("apollo.cassio.q.hitchance", "Q HitChance").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));

                CassioConfig.AddSubMenu(spellQ);
            }
            var spellW = new Menu("W", "apollo.cassio.w");
            {
                spellW.AddSubMenu(new Menu("Modes", "apollo.cassio.w.modes"));
                spellW.SubMenu("apollo.cassio.w.modes").AddItem(new MenuItem("apollo.cassio.w.modes.combo", "Use in Combo").SetValue(true));
                spellW.SubMenu("apollo.cassio.w.modes").AddItem(new MenuItem("apollo.cassio.w.modes.harass", "Use in Harass").SetValue(true));
                spellW.SubMenu("apollo.cassio.w.modes").AddItem(new MenuItem("apollo.cassio.w.modes.laneclear", "Use in LaneClear").SetValue(true));

                var laneClear = new Menu("Laneclear", "apollo.cassio.w.laneclear");
                {
                    laneClear.AddItem(
                        new MenuItem("apollo.cassio.w.laneclear.poi", "Only use on non Poisoned CS").SetValue(false));
                    spellW.AddSubMenu(laneClear);
                }

                spellW.AddItem(new MenuItem("apollo.cassio.w.hitchance", "W HitChance").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));

                CassioConfig.AddSubMenu(spellW);
            }
            var spellE = new Menu("E", "apollo.cassio.e");
            {
                spellE.AddSubMenu(new Menu("Modes", "apollo.cassio.e.modes"));
                spellE.SubMenu("apollo.cassio.e.modes").AddItem(new MenuItem("apollo.cassio.e.modes.combo", "Use in Combo").SetValue(true));
                spellE.SubMenu("apollo.cassio.e.modes").AddItem(new MenuItem("apollo.cassio.e.modes.harass", "Use in Harass").SetValue(true));
                spellE.SubMenu("apollo.cassio.e.modes").AddItem(new MenuItem("apollo.cassio.e.modes.laneclear", "Use in LaneClear").SetValue(true));


                var laneClear = new Menu("Laneclear", "apollo.cassio.e.laneclear");
                {
                    laneClear.AddItem(new MenuItem("apollo.cassio.e.minkill", "Lasthit Minion if not Poisoned").SetValue(false));
                    laneClear.AddItem(new MenuItem("apollo.cassio.e.mincanonkill", "Lasthit Canon Minion if not Poisoned").SetValue(true));

                    spellE.AddSubMenu(laneClear);
                }

                spellE.AddItem(new MenuItem("apollo.cassio.e.kill", "If E Dmg > Target Health").SetValue(true));

                CassioConfig.AddSubMenu(spellE);
            }
            var spellR = new Menu("R", "apollo.cassio.r");
            {
                spellR.AddSubMenu(new Menu("Modes", "apollo.cassio.r.modes"));
                spellR.SubMenu("apollo.cassio.r.modes").AddItem(new MenuItem("apollo.cassio.r.modes.combo", "Use in Combo").SetValue(true));

                var minHit = new Menu("AOE Mode", "apollo.cassio.r.minhit");
                {
                    minHit.AddItem(new MenuItem("apollo.cassio.r.minhit.hit", "Ult if min Enemys Hit").SetValue(new Slider(3, 0, 5)));
                    minHit.AddItem(new MenuItem("apollo.cassio.r.minhit.stunned", "Ult if min Enemys Stunned").SetValue(new Slider(2, 0, 5)));
                    minHit.AddItem(new MenuItem("apollo.cassio.r.minhit.mode", "Mode").SetValue(new StringList((new []{ "Hit", "Stunned", "None" }), 1)));

                    spellR.AddSubMenu(minHit);
                }

                spellR.AddItem(new MenuItem("apollo.cassio.r.kill", "Ult if Target is Killable").SetValue(true));
                spellR.AddItem(new MenuItem("apollo.cassio.r.seltar", "Always Ult/Stun Selected Target").SetValue(true));

                CassioConfig.AddSubMenu(spellR);
            }

            //Delay Menu
            var spellDelay = new Menu("Humanizer", "apollo.cassio.delays");
            {
                var singleSpell = new Menu("Single Spell", "apollo.cassio.delays.single");
                {
                    singleSpell.AddItem(
                        new MenuItem("apollo.cassio.delays.single.q", "Q").SetValue(new Slider(1000, 0, 2000)));
                    singleSpell.AddItem(
                        new MenuItem("apollo.cassio.delays.single.w", "W").SetValue(new Slider(1000, 0, 2000)));
                    singleSpell.AddItem(
                        new MenuItem("apollo.cassio.delays.single.e", "E").SetValue(new Slider(1000, 0, 2000)));
                    spellDelay.AddSubMenu(singleSpell);
                }

                var allSpells = new Menu("All Spells", "apollo.cassio.delays.all");
                {
                    allSpells.AddItem(
                        new MenuItem("apollo.cassio.delays.all.delay", "Foreach Spell").SetValue(new Slider(250, 0, 500)));
                    spellDelay.AddSubMenu(allSpells);
                }
                spellDelay.AddItem(new MenuItem("apollo.cassio.delays.harass", "Delay for Harass").SetValue(false));
                spellDelay.AddItem(
                        new MenuItem("apollo.cassio.delays.mode", "Humanizer Mode").SetValue(
                            new StringList((new[] { "Single Spell", "All Spells", "Disabled" }), 2)));

                CassioConfig.AddSubMenu(spellDelay);
            }

            var misc = new Menu("Misc", "apollo.cassio.misc");
            {
                misc.AddItem(new MenuItem("apollo.cassio.misc.orb", "Disable Attacks in Combo").SetValue(false));

                CassioConfig.AddSubMenu(misc);
            }

            CassioConfig.AddItem(new MenuItem("apollo.cassio.packetcast", "PacketCast").SetValue(false));

            CassioConfig.AddToMainMenu();
        }
    }
}
