using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;


namespace Urgot
{
    internal  class Config
    {
        public static Menu Urgot;
        public static Menu TargetSelectorMenu;
        public static Orbwalking.Orbwalker Orbwalker;
        public static void Init()
        {
            Urgot = new Menu("Apollo's " + ObjectManager.Player.ChampionName, "apollo.urgot", true);

            //Orbwalker
            Urgot.AddSubMenu(new Menu("Orbwalker", "apollo.urgot.orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Urgot.SubMenu("apollo.urgot.orbwalker"));

            //Targetselector
            TargetSelectorMenu = new Menu("Target Selector", "apollo.urgot.targettelector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Urgot.AddSubMenu(TargetSelectorMenu);

            //Combo
            var combo = new Menu("Combo", "apollo.urgot.combo");
            {
                var q = new Menu("Q", "apollo.urgot.combo.q");
                q.AddItem(new MenuItem("apollo.urgot.combo.q.bool", "Use in Combo").SetValue(true));
                q.AddItem(new MenuItem("apollo.urgot.combo.q.pre", "Minimum HitChance").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));
                combo.AddSubMenu(q);

                var w = new Menu("W", "apollo.urgot.combo.w");
                w.AddItem(new MenuItem("apollo.urgot.combo.w.bool", "Use in Combo").SetValue(true));
                w.AddItem(new MenuItem("apollo.urgot.combo.w.onq", "Use on Q").SetValue(true));
                w.AddItem(new MenuItem("apollo.urgot.combo.w.aa", "Use on AA").SetValue(false));
                w.AddItem(new MenuItem("apollo.urgot.combo.w.mana", "Minimum Mana%").SetValue(new Slider(8)));
                combo.AddSubMenu(w);

                var e = new Menu("E", "apollo.urgot.combo.e");
                e.AddItem(new MenuItem("apollo.urgot.combo.e.bool", "Use in Combo").SetValue(true));
                e.AddItem(new MenuItem("apollo.urgot.combo.e.pre", "Minimum HitChance").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));

                combo.AddItem(new MenuItem("apollo.urgot.combo.ignite.bool", "Use Ignite").SetValue(true));

                Urgot.AddSubMenu(combo);
            }

            //Harass
            var harass = new Menu("Harass", "apollo.urgot.harass");
            {
                var q = new Menu("Q", "apollo.urgot.harass.q");
                q.AddItem(new MenuItem("apollo.urgot.harass.q.bool", "Use in Harass").SetValue(true));
                q.AddItem(new MenuItem("apollo.urgot.harass.q.pre", "Minimum HitChance").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 3)));
                harass.AddSubMenu(q);

                var e = new Menu("E", "apollo.urgot.harass.e");
                e.AddItem(new MenuItem("apollo.urgot.harass.e.bool", "Use in Harass").SetValue(true));
                e.AddItem(new MenuItem("apollo.urgot.harass.e.pre", "Minimum HitChance").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 3)));
                harass.AddSubMenu(e);

                harass.AddItem(new MenuItem("apollo.urgot.harass.mana", "Minimum Mana%").SetValue(new Slider(30)));
            }

            //Laneclear
            var laneclear = new Menu("Laneclear", "apollo.urgot.laneclear");
            {
                var q = new Menu("Q", "apollo.urgot.laneclear.q");
                q.AddItem(new MenuItem("apollo.urgot.laneclear.q.bool", "Use in Laneclear").SetValue(true));
                q.AddItem(new MenuItem("apollo.urgot.laneclear.q.lasthit", "Lasthit Minions").SetValue(true));
                laneclear.AddSubMenu(q);

                var e = new Menu("E", "apollo.urgot.laneclear.e");
                e.AddItem(new MenuItem("apollo.urgot.laneclear.e.bool", "Use in Laneclear").SetValue(true));
                e.AddItem(new MenuItem("apollo.urgot.laneclear.e.hit", "Minmum Hit").SetValue(new Slider(3, 1, 5)));
                laneclear.AddSubMenu(e);

                laneclear.AddItem(new MenuItem("apollo.urgot.laneclear.mana", "Minimum Mana%").SetValue(new Slider(30)));
            }

            //Killsteal
            var ks = new Menu("Killsteal", "apollo.urgot.ks");
            {
                ks.AddItem(new MenuItem("apollo.urgot.ks.q.bool", "Use Q").SetValue(true));

                Urgot.AddSubMenu(ks);
            }

            //Interrupter
            var interrupter = new Menu("Interrupter", "apollo.urgot.interrupter");
            {
                interrupter.AddItem(new MenuItem("apollo.urgot.interrupter.r.bool", "Use R").SetValue(true));
                interrupter.AddItem(
                    new MenuItem("apollo.urgot.interrupter.dangerlvl", "DangerLvl").SetValue(
                        new StringList((new[] { "Low", "Medium", "High" }), 2)));

                Urgot.AddSubMenu(interrupter);
            }

            //Misc
            var misc = new Menu("Misc", "apollo.urgot.misc");
            {
                misc.AddItem(new MenuItem("apollo.urgot.misc.autoq", "Auto Q").SetValue(true));
                misc.AddItem(new MenuItem("apollo.urgot.misc.autow", "Auto Shield").SetValue(true));
                misc.AddItem(new MenuItem("apollo.urgot.misc.autor", "Auto R under Tower").SetValue(true));

                Urgot.AddSubMenu(misc);
            }

            //Drawings
            var draw = new Menu("Drawings", "apollo.urgot.draw");
            {
                draw.AddItem(
                    new MenuItem("apollo.urgot.draw.q", "Draw Q Range").SetValue(new Circle(true, Color.AntiqueWhite)));
                draw.AddItem(
                    new MenuItem("apollo.urgot.draw.w", "Draw W Range").SetValue(new Circle(true, Color.AntiqueWhite)));
                draw.AddItem(
                    new MenuItem("apollo.urgot.draw.e", "Draw E Range").SetValue(new Circle(true, Color.AntiqueWhite)));
                draw.AddItem(
                    new MenuItem("apollo.urgot.draw.r", "Draw R Range").SetValue(new Circle(true, Color.AntiqueWhite)));
                draw.AddItem(
                    new MenuItem("apollo.urgot.draw.cd", "Draw on CD").SetValue(new Circle(false, Color.DarkRed)));
                draw.AddItem(
                    new MenuItem("apollo.urgot.draw.ind.bool", "Draw Combo Damage", true).SetValue(true));
                draw.AddItem(
                    new MenuItem("apollo.urgot.draw.ind.fill", "Draw Combo Damage Fill", true).SetValue(
                        new Circle(true, Color.FromArgb(90, 255, 169, 4))));

                Urgot.AddSubMenu(draw);
            }
        }
    }
}
