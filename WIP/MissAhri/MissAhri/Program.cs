using System;

using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace MissAhri
{
    internal class Program
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static Menu AhriConfig;
        private static Orbwalking.Orbwalker Orb;
        private static bool PacketCast;
        private static Spell Q, W, E, R;

        static void Main(string[] args)
        {
            if (args != null)
                CustomEvents.Game.OnGameLoad += OnLoad;
        }

        static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != "Ahri")
                return;

            Game.OnGameUpdate += OnGameUpdate;
            Config();
            Spells();
            Utility.HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Utility.HpBarDamageIndicator.Enabled = true;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += InterrupterOnOnPossibleToInterrupt;
            Drawing.OnDraw += OnDraw;
        }

        static void OnGameUpdate(EventArgs args)
        {
            if (args == null || Player.IsDead || Player.IsRecalling())
                return;

            switch (Orb.ActiveMode)
            {
                    case Orbwalking.OrbwalkingMode.Combo:
                {
                    Combo();
                    break;
                }
                    case Orbwalking.OrbwalkingMode.LaneClear:
                {
                    LaneMode();
                    break;
                }
                    case Orbwalking.OrbwalkingMode.None:
                {
                    //todo Flee soon
                    break;
                }
            }

            PacketCast = AhriConfig.Item("packets.Bool").GetValue<bool>();
        }

        static void Config()
        {
            AhriConfig = new Menu("MissAhri", "MissAhri", true);

            //Orbwalker
            var orbwalker = new Menu("Orbwalker", "orbwalker");
            Orb = new Orbwalking.Orbwalker(orbwalker);
            AhriConfig.AddSubMenu(orbwalker);

            //TargetSelector
            var targetselectormenu = new Menu("TargetSelector", "Common_TargetSelector");
            TargetSelector.AddToMenu(targetselectormenu);
            AhriConfig.AddSubMenu(targetselectormenu);

            //ManaManager
            var manaManager = new Menu("Mana Manager", "ahri.manamanager");
            {
                manaManager.AddItem(new MenuItem("ahri.manamanager.mana", "Harass: Minimum Mana %").SetValue(new Slider(30)));
            }

            var spellQ = new Menu("Q", "Q");
            {
                spellQ.AddSubMenu(new Menu("Modes", "Q.Modes"));
                spellQ.SubMenu("Q.Modes").AddItem(new MenuItem("Q.Combo", "Use in Combo").SetValue(true));
                spellQ.SubMenu("Q.Modes").AddItem(new MenuItem("Q.Harass", "Use in Harass").SetValue(true));

                spellQ.AddItem(new MenuItem("Q.Hitchance", "Q HitChance").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 3)));

                AhriConfig.AddSubMenu(spellQ);
            }
            var spellW = new Menu("W", "W");
            {
                spellW.AddSubMenu(new Menu("Modes", "W.Modes"));
                spellW.SubMenu("W.Modes").AddItem(new MenuItem("W.Combo", "Use in Combo").SetValue(true));
                spellW.SubMenu("W.Modes").AddItem(new MenuItem("W.Harass", "Use in Harass").SetValue(true));

                AhriConfig.AddSubMenu(spellW);
            }
            var spellE = new Menu("E", "E");
            {
                spellE.AddSubMenu(new Menu("Modes", "E.Modes"));
                spellE.SubMenu("E.Modes").AddItem(new MenuItem("E.Combo", "Use in Combo").SetValue(true));
                spellE.SubMenu("E.Modes").AddItem(new MenuItem("E.Harass", "Use in Harass").SetValue(true));

                spellE.AddItem(new MenuItem("E.Hitchance", "E HitChance").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }), 3)));

                AhriConfig.AddSubMenu(spellE);
            }
            var spellR = new Menu("R", "R");
            {
                spellR.AddSubMenu(new Menu("Modes", "R.Modes"));
                spellR.SubMenu("R.Modes").AddItem(new MenuItem("R.Combo", "Use in Combo").SetValue(true));
                spellR.SubMenu("R.Modes").AddItem(new MenuItem("R.Harass", "Use in Harass").SetValue(true));

                AhriConfig.AddSubMenu(spellR);
            }

            //Misc
            var miscMenu = new Menu("Misc", "Misc");
            {
                /*miscMenu.AddSubMenu(new Menu("Flee", "misc.Flee"));
                miscMenu.SubMenu("misc.Flee").AddItem(new MenuItem("misc.Flee.Key", "Flee").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                */

                miscMenu.AddSubMenu(new Menu("Interrupter", "misc.Interrupt"));
                miscMenu.SubMenu("misc.Interrupt").AddItem(new MenuItem("misc.Interrupt.E", "Use E").SetValue(true));

                miscMenu.AddSubMenu(new Menu("AntiGapcloser", "misc.Anitgapcloser"));
                miscMenu.SubMenu("misc.Anitgapcloser").AddItem(new MenuItem("misc.Anitgapcloser.E", "Use E").SetValue(true));

                AhriConfig.AddSubMenu(miscMenu);
            }

            //Drawing
            var drawMenu = new Menu("Drawing", "Drawing");
            {
                /*drawMenu.AddSubMenu(new Menu("Damage Indicator", "draw.Damage"));
                drawMenu.SubMenu("draw.Damage").AddItem(new MenuItem("draw.Damage.Bool", "Draw Combo Damage")).SetValue(true);
                drawMenu.SubMenu("draw.Damage").AddItem(new MenuItem("draw.Damage.Color", "Draw Combo Damage Color")).SetValue(new Circle(true, Color.FromArgb(90, 0, 191, 255)));
                */

                drawMenu.AddSubMenu(new Menu("Spells", "draw.Spells"));
                drawMenu.SubMenu("draw.Spells").AddItem(new MenuItem("draw.Spells.Q", "Draw Q").SetValue(new Circle(true, Color.AntiqueWhite)));
                drawMenu.SubMenu("draw.Spells").AddItem(new MenuItem("draw.Spells.W", "Draw W").SetValue(new Circle(true, Color.AntiqueWhite)));
                drawMenu.SubMenu("draw.Spells").AddItem(new MenuItem("draw.Spells.E", "Draw E").SetValue(new Circle(true, Color.AntiqueWhite)));

                AhriConfig.AddSubMenu(drawMenu);
            }

            //Packets
            AhriConfig.AddItem(new MenuItem("packets.Bool", "Use Packets")).SetValue(false);

            //SumLove
            AhriConfig.AddItem(new MenuItem("lovekappa", "Made for MissSecret").DontSave());

            AhriConfig.AddToMainMenu();
        }

        static void Spells()
        {
            Q = new Spell(SpellSlot.Q, 880);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, 975);
            R = new Spell(SpellSlot.E, 450);

            Q.SetSkillshot(0.25f, 100, 2500, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60, 1500, true, SkillshotType.SkillshotLine);
        }

        static void Combo()
        {
            var t = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);
            var useQ = AhriConfig.Item("Q.Combo").GetValue<bool>();
            var preQ = (HitChance)(AhriConfig.Item("Q.Hitchance").GetValue<StringList>().SelectedIndex + 3);
            var useW = AhriConfig.Item("W.Combo").GetValue<bool>();
            var useE = AhriConfig.Item("E.Combo").GetValue<bool>();
            var preE = (HitChance)(AhriConfig.Item("E.Hitchance").GetValue<StringList>().SelectedIndex + 3);
            var useR = AhriConfig.Item("R.Combo").GetValue<bool>();

            if (useQ && t.IsValidTarget(Q.Range))
                Q.CastIfHitchanceEquals(t, preQ, PacketCast);
            if (useW && t.IsValidTarget(W.Range))
                W.Cast(Player, PacketCast);
            if (useE && t.IsValidTarget(E.Range))
                E.CastIfHitchanceEquals(t, preE, PacketCast);
            if (ComboDamage(t) > t.Health && useR && Player.Distance(t) < 1400)
                R.Cast(Game.CursorPos, PacketCast);
        }

        static void LaneMode()
        {
            var t = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);
            var useQ = AhriConfig.Item("Q.Harass").GetValue<bool>();
            var preQ = (HitChance)(AhriConfig.Item("Q.Hitchance").GetValue<StringList>().SelectedIndex + 3);
            var useE = AhriConfig.Item("E.Harass").GetValue<bool>();
            var preE = (HitChance)(AhriConfig.Item("E.Hitchance").GetValue<StringList>().SelectedIndex + 3);
            var mana = Player.ManaPercentage() > AhriConfig.Item("ahri.manamanager.mana").GetValue<Slider>().Value;

            if (!mana)
                return;

            if (useQ && t.IsValidTarget(Q.Range))
                Q.CastIfHitchanceEquals(t, preQ, PacketCast);
            if (useE && t.IsValidTarget(E.Range))
                E.CastIfHitchanceEquals(t, preE, PacketCast);
        }

        static float ComboDamage(Obj_AI_Hero t)
        {
            double dmg = 0;

            if (Q.IsReady())
            {
                dmg += Player.GetSpellDamage(t, SpellSlot.Q) + Player.GetSpellDamage(t, SpellSlot.Q, 1);
            }

            if (W.IsReady())
            {
                dmg += Player.GetSpellDamage(t, SpellSlot.W);
            }

            if (E.IsReady())
            {
                dmg += Player.GetSpellDamage(t, SpellSlot.E);
            }

            if (R.IsReady())
            {
                dmg += Player.GetSpellDamage(t, SpellSlot.R);
            }

            dmg += dmg;

            return (float)dmg;
        }

        static void InterrupterOnOnPossibleToInterrupt(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!AhriConfig.Item("misc.Interrupt.E").GetValue<bool>() || args.DangerLevel != Interrupter2.DangerLevel.High)
            {
                return;
            }

            E.Cast(sender, PacketCast);
        }

        static void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!AhriConfig.Item("misc.Anitgapcloser.E").GetValue<bool>())
            {
                return;
            }

            E.Cast(gapcloser.Sender, PacketCast);
        }

        static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            var drawQ = AhriConfig.Item("draw.Spells.Q").GetValue<Circle>();
            if (drawQ.Active && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? drawQ.Color : Color.DarkRed);
            }
            var drawW = AhriConfig.Item("draw.Spells.W").GetValue<Circle>();
            if (drawW.Active && W.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? drawW.Color : Color.DarkRed);
            }
            var drawE = AhriConfig.Item("draw.Spells.E").GetValue<Circle>();
            if (drawE.Active && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, E.IsReady() ? drawE.Color : Color.DarkRed);
            }
        }

    }
}
