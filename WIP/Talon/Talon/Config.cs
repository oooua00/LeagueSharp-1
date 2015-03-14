using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Talon
{
    internal class Config
    {
        public static Menu TalonConfig;
        public static Menu TargetSelectorMenu;
        public static Orbwalking.Orbwalker Orbwalker;

        public static void Init()
        {
            TalonConfig = new Menu("Apollo's " + ObjectManager.Player.ChampionName, "apollo.talon", true);

            //Orbwalker
            TalonConfig.AddSubMenu(new Menu("Orbwalker", "apollo.talon.orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(TalonConfig.SubMenu("apollo.talon.orbwalker"));

            //Targetselector
            TargetSelectorMenu = new Menu("Target Selector", "apollo.talon.targettelector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            TalonConfig.AddSubMenu(TargetSelectorMenu);

            //Combo
            var combo = new Menu("Combo", "apollo.talon.combo");
            {

                var q = new Menu("Q", "apollo.talon.combo.q");
                q.AddItem(new MenuItem("apollo.talon.combo.q.bool", "Use in Combo").SetValue(true));
                q.AddItem(
                    new MenuItem("apollo.talon.combo.q.mode", "Mode").SetValue(
                        new StringList((new[] { "After AA", "Before AA" }), 1)));
                combo.AddSubMenu(q);

                var w = new Menu("W", "apollo.talon.combo.w");
                w.AddItem(new MenuItem("apollo.talon.combo.w.bool", "Use in Combo").SetValue(true));
                w.AddItem(
                    new MenuItem("apollo.talon.combo.w.pre", "Minimum HitChance").SetValue(
                        new StringList((new[] { "Low", "Medium", "High", "Very High" }), 2)));
                combo.AddSubMenu(w);

                var e = new Menu("E", "apollo.talon.combo.e");
                e.AddItem(new MenuItem("apollo.talon.combo.e.bool", "Use in Combo").SetValue(true));
                combo.AddSubMenu(e);

                var r = new Menu("R", "apollo.talon.combo.r");
                r.AddItem(new MenuItem("apollo.talon.combo.r.bool", "Use in Combo").SetValue(true));
                r.AddItem(new MenuItem("apollo.talon.combo.r.hit", "Minimum Hit").SetValue(new Slider(3, 1, 5)));
                combo.AddSubMenu(r);

                combo.AddItem(new MenuItem("apollo.talon.combo.ignite.bool", "Use Ignite").SetValue(true));

                TalonConfig.AddSubMenu(combo);
            }

            //Harass
            var harass = new Menu("Harass", "apollo.talon.harass");
            {
                harass.AddItem(new MenuItem("apollo.talon.harass.q.bool", "Use Q").SetValue(true));

                harass.AddItem(new MenuItem("apollo.talon.harass.w.bool", "Use W").SetValue(true));
                harass.AddItem(
                    new MenuItem("apollo.talon.harass.w.pre", "W HitChance").SetValue(
                        new StringList((new[] { "Low", "Medium", "High", "Very High" }), 3)));

                harass.AddItem(new MenuItem("apollo.talon.harass.e.bool", "Use E").SetValue(true));

                harass.AddItem(new MenuItem("apollo.talon.harass.mana", "Minimum Mana").SetValue(new Slider(30)));

                harass.AddItem(
                    new MenuItem("apollo.talon.harass.key", "Harass Toggle").SetValue(
                        new KeyBind('T', KeyBindType.Toggle)));

                TalonConfig.AddSubMenu(harass);
            }

            //Laneclear
            var laneclear = new Menu("Laneclear", "apollo.talon.laneclear");
            {
                laneclear.AddItem(new MenuItem("apollo.talon.laneclear.q.bool", "Use Q").SetValue(true));
                laneclear.AddItem(new MenuItem("apollo.talon.laneclear.w.bool", "Use W").SetValue(true));
                laneclear.AddItem(
                    new MenuItem("apollo.talon.laneclear.w.hit", "Minimum hit by W").SetValue(new Slider(3, 1, 10)));
                laneclear.AddItem(new MenuItem("apollo.talon.laneclear.mana", "Minimum Mana").SetValue(new Slider(30)));

                TalonConfig.AddSubMenu(laneclear);
            }

            //Killsteal
            var killsteal = new Menu("Killsteal", "apollo.talon.ks");
            {
                killsteal.AddItem(new MenuItem("apollo.talon.ks.w.bool", "Use W").SetValue(true));
                TalonConfig.AddSubMenu(killsteal);
            }

            //Drawings
            var draw = new Menu("Drawings", "apollo.talon.draw");
            {
                draw.AddItem(
                    new MenuItem("apollo.talon.draw.w", "Draw W Range").SetValue(new Circle(true, Color.AntiqueWhite)));
                draw.AddItem(
                    new MenuItem("apollo.talon.draw.e", "Draw E Range").SetValue(new Circle(true, Color.AntiqueWhite)));
                draw.AddItem(
                    new MenuItem("apollo.talon.draw.r", "Draw R Range").SetValue(new Circle(true, Color.AntiqueWhite)));
                draw.AddItem(
                    new MenuItem("apollo.talon.draw.cd", "Draw on CD").SetValue(new Circle(false, Color.DarkRed)));
                draw.AddItem(
                    new MenuItem("apollo.talon.draw.ind.bool", "Draw Combo Damage", true).SetValue(true));
                draw.AddItem(
                    new MenuItem("apollo.talon.draw.ind.fill", "Draw Combo Damage Fill", true).SetValue(
                        new Circle(true, Color.FromArgb(90, 255, 169, 4))));

                TalonConfig.AddSubMenu(draw);
            }

            //Packets
            TalonConfig.AddItem(new MenuItem("apollo.talon.packets.bool", "Use Packets").SetValue(false));
        }
    }
}
