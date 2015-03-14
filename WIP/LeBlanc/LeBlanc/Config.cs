using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace LeBlanc
{
    internal class Config
    {
        public static Menu LeBlanc;
        public static Menu TargetSelectorMenu;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Init()
        {
            LeBlanc = new Menu("Apollo's " + ObjectManager.Player.ChampionName, "apollo.leblanc", true);

            //Orbwalker
            LeBlanc.AddSubMenu(new Menu("Orbwalker", "apollo.leblanc.orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(LeBlanc.SubMenu("apollo.leblanc.orbwalker"));

            //Targetselector
            TargetSelectorMenu = new Menu("Target Selector", "apollo.leblanc.targettelector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            LeBlanc.AddSubMenu(TargetSelectorMenu);

            //Combo
            var combo = new Menu("Combo", "apollo.leblanc.combo");
            {
                var q = new Menu("Q", "apollo.leblanc.combo.q");
                q.AddItem(new MenuItem("apollo.leblanc.combo.q.bool", "Use in Combo").SetValue(true));
                combo.AddSubMenu(q);

                var w = new Menu("W", "apollo.leblanc.combo.w");
                w.AddItem(new MenuItem("apollo.leblanc.combo.w.bool", "Use in Combo").SetValue(true));
                w.AddItem(
                    new MenuItem("apollo.leblanc.combo.w.pre", "Minimum HitChance").SetValue(
                        new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));
                combo.AddSubMenu(w);

                var e = new Menu("E", "apollo.leblanc.combo.e");
                e.AddItem(new MenuItem("apollo.leblanc.combo.e.bool", "Use in Combo").SetValue(true));
                e.AddItem(
                    new MenuItem("apollo.leblanc.combo.e.pre", "Minimum HitChance").SetValue(
                        new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));
                combo.AddSubMenu(e);

                var r = new Menu("R", "apollo.leblanc.combo.r");
                r.AddItem(new MenuItem("apollo.leblanc.combo.r.bool", "Use in Combo").SetValue(true));

                var rw = new Menu("W", "apollo.leblanc.combo.r.w");
                rw.AddItem(
                    new MenuItem("apollo.leblanc.combo.r.w.pre", "Minimum HitChance").SetValue(
                        new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));
                r.AddSubMenu(rw);

                var re = new Menu("E", "apollo.leblanc.combo.r.e");
                re.AddItem(
                    new MenuItem("apollo.leblanc.combo.r.e.pre", "Minimum HitChance").SetValue(
                        new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));
                r.AddSubMenu(re);

                combo.AddSubMenu(r);

                combo.AddItem(new MenuItem("apollo.leblanc.combo.ignite.bool", "Use Ignite").SetValue(true));

                LeBlanc.AddSubMenu(combo);
            }

            //Harass
            var harass = new Menu("Harass", "apollo.leblanc.harass");
            {
                var q = new Menu("Q", "apollo.leblanc.harass.q");
                q.AddItem(new MenuItem("apollo.leblanc.harass.q.bool", "Use in Harass").SetValue(true));
                harass.AddSubMenu(q);

                var w = new Menu("W", "apollo.leblanc.harass.w");
                w.AddItem(new MenuItem("apollo.leblanc.harass.w.bool", "Use in Harass").SetValue(true));
                w.AddItem(new MenuItem("apollo.leblanc.harass.w2.bool", "Use second W in Harass").SetValue(true));
                w.AddItem(
                    new MenuItem("apollo.leblanc.harass.w.pre", "Minimum HitChance").SetValue(
                        new StringList((new[] { "Low", "Medium", "High", "Very High" }), 3)));
                harass.AddSubMenu(w);

                var e = new Menu("E", "apollo.leblanc.harass.e");
                e.AddItem(new MenuItem("apollo.leblanc.harass.e.bool", "Use in Harass").SetValue(true));
                e.AddItem(
                    new MenuItem("apollo.leblanc.harass.e.pre", "Minimum HitChance").SetValue(
                        new StringList((new[] { "Low", "Medium", "High", "Very High" }), 3)));
                harass.AddSubMenu(e);

                harass.AddItem(new MenuItem("apollo.leblanc.harass.mana", "Minimum Mana%").SetValue(new Slider(30)));
                harass.AddItem(
                    new MenuItem("apollo.leblanc.harass.key", "Toggle Key").SetValue(
                        new KeyBind('T', KeyBindType.Toggle)));

                LeBlanc.AddSubMenu(harass);
            }

            //Laneclear
            var laneclear = new Menu("Laneclear", "apollo.leblanc.laneclear");
            {
                var q = new Menu("Q", "apollo.leblanc.laneclear.q");
                q.AddItem(new MenuItem("apollo.leblanc.laneclear.q.bool", "Use in Laneclear").SetValue(true));
                laneclear.AddSubMenu(q);

                var w = new Menu("W", "apollo.leblanc.laneclear.w");
                w.AddItem(new MenuItem("apollo.leblanc.laneclear.w.bool", "Use in Laneclear").SetValue(true));
                w.AddItem(new MenuItem("apollo.leblanc.laneclear.w2.bool", "Use second W in Laneclear").SetValue(true));
                w.AddItem(new MenuItem("apollo.leblanc.laneclear.w.hit", "Minimum Hit").SetValue(new Slider(3, 1, 5)));
                laneclear.AddSubMenu(w);

                laneclear.AddItem(
                    new MenuItem("apollo.leblanc.laneclear.mana", "Minimum Mama%").SetValue(new Slider(30)));

                LeBlanc.AddSubMenu(laneclear);
            }

            //Killsteal
            var killsteal = new Menu("Killsteal", "apollo.leblanc.ks");
            {
                killsteal.AddItem(new MenuItem("apollo.leblanc.ks.q.bool", "Use Q").SetValue(true));
                killsteal.AddItem(new MenuItem("apollo.leblanc.ks.w.bool", "Use W").SetValue(true));

                LeBlanc.AddSubMenu(killsteal);
            }

            //AnitGap && Interrupter
            var anti = new Menu("AnitGapcloser & Interrupter", "apollo.leblanc.anti");
            {
                var anitgap = new Menu("AnitGapcloser", "apollo.leblanc.anti.gapcloser");
                {
                    var e = new Menu("E", "apollo.leblanc.anti.gapcloser.e");
                    e.AddItem(
                        new MenuItem("apollo.leblanc.anti.gapcloser.e.bool", "Use as AnitGapcloser").SetValue(true));
                    e.AddItem(
                        new MenuItem("apollo.leblanc.anti.gapcloser.e.pre", "Minimum HitChance").SetValue(
                            new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));
                    anitgap.AddSubMenu(e);

                    var r = new Menu("R(E)", "apollo.leblanc.anti.gapcloser.r");
                    r.AddItem(
                        new MenuItem("apollo.leblanc.anti.gapcloser.r.bool", "Use as AnitGapcloser").SetValue(true));
                    r.AddItem(
                        new MenuItem("apollo.leblanc.anti.gapcloser.r.pre", "Minimum HitChance").SetValue(
                            new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));
                    anitgap.AddSubMenu(r);

                    anti.AddSubMenu(anitgap);
                }
                var interrupter = new Menu("Interrupter", "apollo.leblanc.anti.interrupter");
                {
                    var e = new Menu("E", "apollo.leblanc.anti.interrupter.e");
                    e.AddItem(
                        new MenuItem("apollo.leblanc.anti.interrupter.e.bool", "Use as Interrupter").SetValue(true));
                    e.AddItem(
                        new MenuItem("apollo.leblanc.anti.interrupter.e.pre", "Minimum HitChance").SetValue(
                            new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));
                    interrupter.AddSubMenu(e);

                    var r = new Menu("R(E)", "apollo.leblanc.anti.interrupter.r");
                    r.AddItem(
                        new MenuItem("apollo.leblanc.anti.interrupter.r.bool", "Use as Interrupter").SetValue(true));
                    r.AddItem(
                        new MenuItem("apollo.leblanc.anti.interrupter.r.pre", "Minimum HitChance").SetValue(
                            new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));
                    interrupter.AddSubMenu(r);

                    interrupter.AddItem(
                        new MenuItem("apollo.leblanc.anit.interrupter.dangerlvl", "Minimum Danger Lvl").SetValue(
                            new StringList((new[] { "Low", "Medium", "High" }), 2)));

                    anti.AddSubMenu(interrupter);
                }

                LeBlanc.AddSubMenu(anti);
            }

            //Flee
            var flee = new Menu("Flee", "apollo.leblanc.flee");
            {
                var w = new Menu("W", "apollo.leblanc.flee.w");
                w.AddItem(new MenuItem("apollo.leblanc.flee.w.bool", "Use in Flee").SetValue(true));
                flee.AddSubMenu(w);

                var e = new Menu("E", "apollo.leblanc.flee.e");
                e.AddItem(new MenuItem("apollo.leblanc.flee.e.bool", "Use in Flee").SetValue(true));
                e.AddItem(
                    new MenuItem("apollo.leblanc.flee.e.pre", "Minimum HitChance").SetValue(
                        new StringList((new[] { "Low", "Medium", "High", "Very High" }), 1)));
                flee.AddSubMenu(e);

                var r = new Menu("R", "apollo.leblanc.flee.r");
                r.AddItem(new MenuItem("apollo.leblanc.flee.r.w.bool", "Use in Flee (RW)").SetValue(false));
                flee.AddSubMenu(r);

                flee.AddItem(
                    new MenuItem("apollo.leblanc.flee.key", "Hotkey").SetValue(new KeyBind('A', KeyBindType.Press)));

                LeBlanc.AddSubMenu(flee);
            }

            //Drawings
            var draw = new Menu("Drawings", "apollo.leblanc.draw");
            {
                draw.AddItem(
                    new MenuItem("apollo.leblanc.draw.q", "Draw Q Range").SetValue(new Circle(true, Color.AntiqueWhite)));
                draw.AddItem(
                    new MenuItem("apollo.leblanc.draw.w", "Draw W Range").SetValue(new Circle(true, Color.AntiqueWhite)));
                draw.AddItem(
                    new MenuItem("apollo.leblanc.draw.e", "Draw E Range").SetValue(new Circle(true, Color.AntiqueWhite)));
                draw.AddItem(
                    new MenuItem("apollo.leblanc.draw.cd", "Draw on CD").SetValue(new Circle(false, Color.DarkRed)));
                draw.AddItem(
                    new MenuItem("apollo.leblanc.draw.ind.bool", "Draw Combo Damage", true).SetValue(true));
                draw.AddItem(
                    new MenuItem("apollo.leblanc.draw.ind.fill", "Draw Combo Damage Fill", true).SetValue(
                        new Circle(true, Color.FromArgb(90, 255, 169, 4))));

                LeBlanc.AddSubMenu(draw);
            }

            //Misc
            var misc = new Menu("Misc", "apollo.leblanc.misc");
            {
                var secondW = new Menu("Second W", "apollo.leblanc.misc.2w");
                secondW.AddItem(new MenuItem("apollo.leblanc.misc.2w.mouseover.bool", "Use if Mouse over").SetValue(true));
                secondW.AddItem(
                    new MenuItem("apollo.leblanc.misc.2w.mouseover.width", "Mouse over zone width").SetValue(
                        new Slider(70, 1)));
                misc.AddSubMenu(secondW);

                var clone = new Menu("Clone", "apollo.leblanc.misc.clone");
                clone.AddItem(new MenuItem("apollo.leblanc.misc.clone.move.bool", "Use Clone").SetValue(true));
                misc.AddSubMenu(clone);

                LeBlanc.AddSubMenu(misc);
            }

            //PacketCast
            {
                LeBlanc.AddItem(new MenuItem("apollo.leblanc.packetcast", "PacketCast").SetValue(false));
            }

            LeBlanc.AddToMainMenu();
        }
    }
}
