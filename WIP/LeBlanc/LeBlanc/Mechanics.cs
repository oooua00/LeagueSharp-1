using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc
{
    internal class Mechanics
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Dictionary<SpellSlot, Spell> Spell = Helper.Spells.Spell;
        private static readonly bool PacketCast = Config.LeBlanc.Item("apollo.leblanc.packetcast").GetValue<bool>();
        public static readonly SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");

        public static void Init()
        {
            Game.OnUpdate += OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AnitGapcloser;
            Interrupter2.OnInterruptableTarget += OnPossibleToInterrupt;
        }
        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling() || args == null)
                return;

            Killsteal();
            Misc();

            var harassKey = Config.LeBlanc.Item("apollo.leblanc.harass.key").GetValue<KeyBind>();
            if (harassKey.Active)
                Harass();

            //Notifications
            Notifications.AddNotification("Auto Harass: " + harassKey.Active.ToString(), 1, false);
            Notifications.AddNotification("PacketCast: " + PacketCast.ToString(), 1, false);


            switch (Config.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                {
                    Combo();
                    break;
                }
                case Orbwalking.OrbwalkingMode.LaneClear:
                {
                    Laneclear();
                    Jungleclear();
                    break;
                }
                case Orbwalking.OrbwalkingMode.Mixed:
                {
                    Harass();
                    break;
                }
                case Orbwalking.OrbwalkingMode.None:
                {
                    if (Config.LeBlanc.Item("apollo.leblanc.flee.key").GetValue<KeyBind>().Active)
                        Flee();
                    break;
                }
            }
        }

        private static void Combo()
        {
            var t = GetTarget(Spell[SpellSlot.E].Range, TargetSelector.DamageType.Magical);
            var rName = Spell[SpellSlot.R].Instance.Name;
            var wName = Spell[SpellSlot.W].Instance.Name;

            if (!t.IsValidTarget())
                return;

            if (Helper.SpellDamage.R.W(t) > 2 * Helper.SpellDamage.R.Q(t) && rName != Helper.Spells.Name.R.W2 &&
                Helper.Use.R.Bool(Orbwalking.OrbwalkingMode.Combo) && Spell[SpellSlot.R].IsReady())
            {

                if (rName == Helper.Spells.Name.R.W)
                {
                    Helper.Cast.R.W(t, Helper.GetHitchance.R.W(Orbwalking.OrbwalkingMode.Combo));
                }
                else if ((!Spell[SpellSlot.W].IsReady() || wName == Helper.Spells.Name.W2))
                {
                    if (((t.Health - ComboDmg.Get(t)) / t.MaxHealth) >= .4 && rName == Helper.Spells.Name.R.E)
                    {
                        Helper.Cast.R.E(t, Helper.GetHitchance.R.E(Orbwalking.OrbwalkingMode.Combo));
                    }
                    else if (rName == Helper.Spells.Name.R.Q)
                    {
                        Helper.Cast.R.Q(t);
                    }
                }
            }
            else if (rName != Helper.Spells.Name.R.W2 && Helper.Use.R.Bool(Orbwalking.OrbwalkingMode.Combo) && Spell[SpellSlot.R].IsReady())
            {
                if (rName == Helper.Spells.Name.R.Q)
                {
                    Helper.Cast.R.Q(t);
                }
                else if (!Spell[SpellSlot.Q].IsReady())
                {
                    if (((t.Health - ComboDmg.Get(t)) / t.MaxHealth) >= .4 && rName == Helper.Spells.Name.R.E)
                    {
                        Helper.Cast.R.E(t, Helper.GetHitchance.R.E(Orbwalking.OrbwalkingMode.Combo));
                    }
                    else if (rName == Helper.Spells.Name.R.W)
                    {
                        Helper.Cast.R.W(t, Helper.GetHitchance.R.W(Orbwalking.OrbwalkingMode.Combo));
                    }
                }
            }

            if (Helper.Use.Q(Orbwalking.OrbwalkingMode.Combo))
            {
                Helper.Cast.Q(t);
            }
            if (Helper.Use.W(Orbwalking.OrbwalkingMode.Combo) && wName != Helper.Spells.Name.W2 &&
                !Spell[SpellSlot.Q].IsReady())
            {
                Helper.Cast.W(t, Helper.GetHitchance.W(Orbwalking.OrbwalkingMode.Combo));
            }
            if (Helper.Use.E(Orbwalking.OrbwalkingMode.Combo))
            {
                Helper.Cast.E(t, Helper.GetHitchance.E(Orbwalking.OrbwalkingMode.Combo));
            }
            if (rName == Helper.Spells.Name.R.W2)
            {
                Helper.Cast.R.W2();
            }
        }
        private static void Laneclear()
        {
            var minions = MinionManager.GetMinions(Player.ServerPosition, Spell[SpellSlot.Q].Range);
            var mana = Player.ManaPercent > Config.LeBlanc.Item("apollo.leblanc.laneclear.mana").GetValue<Slider>().Value;

            if (Helper.Use.W2(Orbwalking.OrbwalkingMode.LaneClear) &&
                Spell[SpellSlot.W].Instance.Name == Helper.Spells.Name.W2)
            {
                Helper.Cast.W2();
            }

            if (minions == null || !mana)
                return;

            if (Helper.Use.Q(Orbwalking.OrbwalkingMode.LaneClear) && Spell[SpellSlot.Q].IsReady())
            {
                var qMinion =
                    minions.Where(
                        h =>
                            Helper.GetHealthPrediction.From(Player, h, SpellSlot.W, Helper.SpellDamage.Q(h), true) &&
                            Helper.GetHealthPrediction.From(Player, h, SpellSlot.W, 0, false))
                        .OrderBy(h => h.Health)
                        .FirstOrDefault();
                if (qMinion != null)
                {
                    Orbwalking.AfterAttack += (unit, target) =>
                    {
                        Helper.Cast.Q(qMinion);
                    };
                }
            }

            if (Helper.Use.W(Orbwalking.OrbwalkingMode.LaneClear) && Spell[SpellSlot.W].IsReady() &&
                Spell[SpellSlot.W].Instance.Name != Helper.Spells.Name.W2)
            {
                var farmLoc =
                    MinionManager.GetBestCircularFarmLocation(
                        minions.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.W].Width,
                        Spell[SpellSlot.W].Range);
                var minHit = Config.LeBlanc.Item("apollo.leblanc.laneclear.w.hit").GetValue<Slider>().Value;

                if (farmLoc.MinionsHit >= minHit)
                    Spell[SpellSlot.W].Cast(farmLoc.Position, PacketCast);
            }
        }
        private static void Jungleclear()
        {
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, Spell[SpellSlot.W].Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            var mana = Player.ManaPercent > Config.LeBlanc.Item("apollo.leblanc.laneclear.mana").GetValue<Slider>().Value;

            if (Helper.Use.W2(Orbwalking.OrbwalkingMode.LaneClear) &&
                Spell[SpellSlot.W].Instance.Name == Helper.Spells.Name.W2)
            {
                Helper.Cast.W2();
            }

            if (minions == null || !mana)
                return;

            if (Helper.Use.Q(Orbwalking.OrbwalkingMode.LaneClear))
            {
                Helper.Cast.Q(minions.FirstOrDefault());
            }
            if (Helper.Use.W(Orbwalking.OrbwalkingMode.LaneClear) && !Spell[SpellSlot.Q].IsReady() &&
                Spell[SpellSlot.W].Instance.Name != Helper.Spells.Name.W2)
            {
                Spell[SpellSlot.W].Cast(minions.FirstOrDefault(), PacketCast);
            }
        }
        private static void Harass()
        {
            var t = TargetSelector.GetTarget(Player, Spell[SpellSlot.E].Range, TargetSelector.DamageType.Magical);
            var wName = Spell[SpellSlot.W].Instance.Name;
            var mana = Player.ManaPercent > Config.LeBlanc.Item("apollo.leblanc.harass.mana").GetValue<Slider>().Value;

            if (!t.IsValidTarget() && !mana)
                return;

            if (Helper.Use.Q(Orbwalking.OrbwalkingMode.Mixed))
            {
                Helper.Cast.Q(t);
            }
            if (!Spell[SpellSlot.Q].IsReady() && Helper.Use.W(Orbwalking.OrbwalkingMode.Mixed) &&
                wName != Helper.Spells.Name.W2)
            {
                Helper.Cast.W(t, Helper.GetHitchance.W(Orbwalking.OrbwalkingMode.Mixed));
            }
            if (Helper.Use.E(Orbwalking.OrbwalkingMode.Mixed))
            {
                Helper.Cast.E(t, Helper.GetHitchance.E(Orbwalking.OrbwalkingMode.Mixed));
            }
            if (Helper.Use.W2(Orbwalking.OrbwalkingMode.Mixed) && wName == Helper.Spells.Name.W2)
            {
                Helper.Cast.W2();
            }
        }
        private static void Flee()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            
            var t =
                HeroManager.Enemies.Where(h => h.IsValidTarget(Spell[SpellSlot.E].Range))
                    .OrderBy(h => Player.Distance(h))
                    .FirstOrDefault();
            var pos = Player.ServerPosition.Extend(Game.CursorPos, Spell[SpellSlot.W].Range);
            var rName = Spell[SpellSlot.R].Instance.Name;
            var wName = Spell[SpellSlot.W].Instance.Name;

            if (t.IsValidTarget() && Spell[SpellSlot.E].IsReady() && Helper.Use.E(Orbwalking.OrbwalkingMode.None))
            {
                Helper.Cast.E(t, Helper.GetHitchance.E(Orbwalking.OrbwalkingMode.None));
            }
            if (NavMesh.GetCollisionFlags(pos.X, pos.Y) != CollisionFlags.Wall &&
                NavMesh.GetCollisionFlags(pos.X, pos.Y) != CollisionFlags.Building)
            {
                if (Spell[SpellSlot.W].IsReady() && Helper.Use.W(Orbwalking.OrbwalkingMode.None) &&
                    wName != Helper.Spells.Name.W2)
                {
                    Spell[SpellSlot.W].Cast(pos, PacketCast);
                }
                if (Spell[SpellSlot.R].IsReady() && Helper.Use.R.W(Orbwalking.OrbwalkingMode.None) &&
                    rName != Helper.Spells.Name.R.W2 && rName == Helper.Spells.Name.R.W)
                {
                    Spell[SpellSlot.R].Cast(pos, PacketCast);
                }
            }
        }
        private static void Killsteal()
        {
            var t =
                HeroManager.Enemies.Where(
                    h =>
                        h.IsValidTarget(Spell[SpellSlot.Q].Range) &&
                        Helper.GetHealthPrediction.From(
                            Player, h, SpellSlot.W, 2 * Helper.SpellDamage.Q(h) + Helper.SpellDamage.W(h), true) &&
                        Helper.GetHealthPrediction.From(Player, h, SpellSlot.W, 0, false)).OrderBy(h => h.Health);
            var useQ = Config.LeBlanc.Item("apollo.leblanc.ks.q.bool").GetValue<bool>();
            var useW = Config.LeBlanc.Item("apollo.leblanc.ks.w.bool").GetValue<bool>();
            var wName = Spell[SpellSlot.W].Instance.Name;

            if (t != null)
            {
                if (t.FirstOrDefault().IsValidTarget(Spell[SpellSlot.W].Range) && useQ && useW &&
                    Spell[SpellSlot.Q].IsReady() && Spell[SpellSlot.W].IsReady() && wName != Helper.Spells.Name.W2)
                {
                    Helper.Cast.Q(t.FirstOrDefault());
                    if (!Spell[SpellSlot.Q].IsReady())
                        Helper.Cast.W(t.FirstOrDefault(), HitChance.High);
                    if (wName == Helper.Spells.Name.W2)
                        Helper.Cast.W2();
                }
                else
                {
                    var qT =
                        t.Where(
                            h =>
                                Helper.GetHealthPrediction.From(Player, h, SpellSlot.W, Helper.SpellDamage.Q(h), true) &&
                                Helper.GetHealthPrediction.From(Player, h, SpellSlot.W, 0, false))
                            .OrderBy(h => h.Health).FirstOrDefault();
                    var wT =
                        t.Where(
                            h =>
                                h.IsValidTarget(Spell[SpellSlot.W].Range) &&
                                Helper.GetHealthPrediction.From(Player, h, SpellSlot.W, Helper.SpellDamage.W(h), true) &&
                                Helper.GetHealthPrediction.From(Player, h, SpellSlot.W, 0, false))
                            .OrderBy(h => h.Health).FirstOrDefault();

                    if (wT != null && useW && Spell[SpellSlot.W].IsReady() && wName != Helper.Spells.Name.W2)
                    {
                        Helper.Cast.W(wT, HitChance.High);
                        if (wName == Helper.Spells.Name.W2)
                            Helper.Cast.W2();
                    }
                    else if (qT != null && useQ && Spell[SpellSlot.Q].IsReady())
                    {
                        Helper.Cast.Q(qT);
                    }
                }
            }
        }
        private static void Misc()
        {
            if (Config.LeBlanc.Item("apollo.leblanc.misc.2w.mouseover.bool").GetValue<bool>() &&
                Helper.Objects.SecondW.Object != null)
            {
                var width = Config.LeBlanc.Item("apollo.leblanc.misc.2w.mouseover.width").GetValue<Slider>().Value;
                var exPos = Helper.Objects.SecondW.Object.Position.Extend(Game.CursorPos, width);
                var exPosDis = Helper.Objects.SecondW.Object.Position.Distance(exPos);
                var cursorDis = Helper.Objects.SecondW.Object.Position.Distance(Game.CursorPos);
                if (Spell[SpellSlot.W].Instance.Name == Helper.Spells.Name.W2 && cursorDis < exPosDis)
                {
                    Helper.Cast.W2();
                }
            }

            var pet = Helper.Objects.Clone.Pet;
            if (Config.LeBlanc.Item("apollo.leblanc.misc.clone.move.bool").GetValue<bool>() && pet != null &&
                pet.IsValid && !pet.IsDead && pet.Health > 1)
            {
                var t =
                    HeroManager.Enemies.Where(h => h.IsValidTarget(1000)).OrderBy(h => h.Distance(Player)).FirstOrDefault();
                if (t != null && pet.CanAttack)
                    pet.IssueOrder(GameObjectOrder.AutoAttackPet, t);
            }
        }
        private static void OnPossibleToInterrupt(Obj_AI_Base unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            var dangerLvl =
                (Interrupter2.DangerLevel)
                    (Config.LeBlanc.Item("apollo.leblanc.anit.interrupter.dangerlvl")
                        .GetValue<StringList>()
                        .SelectedIndex + 2);

            if (args.DangerLevel < dangerLvl || unit == null)
                return;

            var useE = Config.LeBlanc.Item("apollo.leblanc.anti.interrupter.e.bool").GetValue<bool>();
            var useR = Config.LeBlanc.Item("apollo.leblanc.anti.interrupter.r.bool").GetValue<bool>();
            var preE = (HitChance)(Config.LeBlanc.Item("apollo.leblanc.anti.interrupter.e.pre").GetValue<StringList>().SelectedIndex + 3);
            var preR = (HitChance)(Config.LeBlanc.Item("apollo.leblanc.anti.interrupter.r.pre").GetValue<StringList>().SelectedIndex + 3);

            if (useR && Spell[SpellSlot.R].Instance.Name == Helper.Spells.Name.R.E)
            {
                Spell[SpellSlot.R].CastIfHitchanceEquals(unit, preR);
            }
            else if (useE)
            {
                Spell[SpellSlot.E].CastIfHitchanceEquals(unit, preE);
            }
        }
        private static void AnitGapcloser(ActiveGapcloser gapcloser)
        {
            var useE = Config.LeBlanc.Item("apollo.leblanc.anti.gapcloser.e.bool").GetValue<bool>();
            var useR = Config.LeBlanc.Item("apollo.leblanc.anti.gapcloser.r.bool").GetValue<bool>();
            var preE = (HitChance)(Config.LeBlanc.Item("apollo.leblanc.anti.gapcloser.e.pre").GetValue<StringList>().SelectedIndex + 3);
            var preR = (HitChance)(Config.LeBlanc.Item("apollo.leblanc.anti.gapcloser.r.pre").GetValue<StringList>().SelectedIndex + 3);

            if (useE)
            {
                Spell[SpellSlot.E].CastIfHitchanceEquals(gapcloser.Sender, preE);
            }
            if (useR && Spell[SpellSlot.R].Instance.Name == Helper.Spells.Name.R.E)
            {
                Spell[SpellSlot.R].CastIfHitchanceEquals(gapcloser.Sender, preR);
            }
        }
        private static Obj_AI_Hero GetTarget(float vDefaultRange = 0,
            TargetSelector.DamageType vDefaultDamageType = TargetSelector.DamageType.Physical)
        {
            if (Math.Abs(vDefaultRange) < 0.00001)
                vDefaultRange = Spell[SpellSlot.W].Range;

            if (!Config.LeBlanc.Item("AssassinActive").GetValue<bool>())
                return TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType);

            var assassinRange = Config.LeBlanc.Item("AssassinSearchRange").GetValue<Slider>().Value;

            var vEnemy =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        enemy =>
                            enemy.Team != ObjectManager.Player.Team && !enemy.IsDead && enemy.IsVisible &&
                            Config.LeBlanc.Item("Assassin" + enemy.ChampionName) != null &&
                            Config.LeBlanc.Item("Assassin" + enemy.ChampionName).GetValue<bool>() &&
                            ObjectManager.Player.Distance(enemy) < assassinRange);

            if (Config.LeBlanc.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex == 1)
            {
                vEnemy = (from vEn in vEnemy select vEn).OrderByDescending(vEn => vEn.MaxHealth);
            }

            Obj_AI_Hero[] objAiHeroes = vEnemy as Obj_AI_Hero[] ?? vEnemy.ToArray();

            Obj_AI_Hero t = !objAiHeroes.Any()
                ? TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType)
                : objAiHeroes[0];

            return t;
        }
    }
}
