using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Talon
{
    internal class Mechanics
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;
        private static readonly bool PacketCast = Config.TalonConfig.Item("apollo.talon.packets.bool").GetValue<bool>();
        public static readonly SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");

        public static void Init()
        {
            Game.OnUpdate += OnUpdate;
        }
        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling() || args == null)
                return;

            Killsteal();

            var harassKey = Config.TalonConfig.Item("apollo.talon.harass.key").GetValue<KeyBind>();
            if (harassKey.Active)        
                Harass();

            //Notifications
            //Notifications.AddNotification("Auto Harass: " + harassKey.Active.ToString(), 1, false);
            //Notifications.AddNotification("PacketCast: " + PacketCast.ToString(), 1, false);


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
            }
        }
        private static void Combo()
        {
            var t = GetTarget(Spell[SpellSlot.W].Range);

            if (!t.IsValidTarget())
                return;

            if (IgniteSlot != SpellSlot.Unknown && IgniteSlot.IsReady() &&
                Config.TalonConfig.Item("apollo.talon.combo.ignite.bool").GetValue<bool>() &&
                t.Health < Damages.ComboDmg(t))
            {
                Player.Spellbook.CastSpell(IgniteSlot, t);
            }

            if (Spell[SpellSlot.Q].IsReady() && Helper.Use.Q(Orbwalking.OrbwalkingMode.Combo))
            {
                if (Config.TalonConfig.Item("apollo.talon.combo.q.mode").GetValue<StringList>().SelectedIndex == 1)
                {
                    Orbwalking.BeforeAttack += args =>
                    { Spell[SpellSlot.Q].Cast(PacketCast); };
                }
                else
                {
                    Orbwalking.AfterAttack += (unit, target) =>
                    {
                        if (Orbwalking.InAutoAttackRange(target))
                        {
                            Spell[SpellSlot.Q].Cast(PacketCast);
                            Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    };
                }
            }

            if (Spell[SpellSlot.W].IsReady() && Helper.Use.W(Orbwalking.OrbwalkingMode.Combo))
            {
                var pred = Spell[SpellSlot.W].GetPrediction(t);
                if (pred.Hitchance >= Helper.GetHitchance.W(Orbwalking.OrbwalkingMode.Combo))
                {
                    Helper.Cast.W(t, pred.UnitPosition.To2D(), PacketCast);
                }
            }

            if (Spell[SpellSlot.E].IsReady() && Helper.Use.E(Orbwalking.OrbwalkingMode.Combo) &&
                t.IsValidTarget(Spell[SpellSlot.E].Range))
            {
                Spell[SpellSlot.E].CastOnUnit(t, PacketCast);
            }

            if (Spell[SpellSlot.R].IsReady() && Helper.Use.R(Orbwalking.OrbwalkingMode.Combo) &&
                t.IsValidTarget(Spell[SpellSlot.R].Range) && t.Health < Damages.ComboDmg(t))
            {
                Spell[SpellSlot.R].Cast(PacketCast);
            }
        }
        private static void Harass()
        {
            var t = TargetSelector.GetTarget(Player, Spell[SpellSlot.W].Range, TargetSelector.DamageType.Physical);
            var mana = Player.ManaPercent > Config.TalonConfig.Item("apollo.talon.harass.mana").GetValue<Slider>().Value;

            if (!t.IsValidTarget() || !mana)
                return;

            if (Spell[SpellSlot.Q].IsReady() && Helper.Use.Q(Orbwalking.OrbwalkingMode.Mixed))
            {
                Orbwalking.BeforeAttack += args =>
                {
                    if (!args.Unit.IsMinion)
                        Spell[SpellSlot.Q].Cast(PacketCast);
                };
            }
            if (Spell[SpellSlot.W].IsReady() && Helper.Use.W(Orbwalking.OrbwalkingMode.Mixed))
            {
                var pre = Spell[SpellSlot.W].GetPrediction(t);
                if (pre.Hitchance >= Helper.GetHitchance.W(Orbwalking.OrbwalkingMode.Mixed))
                    Spell[SpellSlot.W].Cast(t, PacketCast);
            }
            if (Spell[SpellSlot.E].IsReady() && Helper.Use.E(Orbwalking.OrbwalkingMode.Mixed))
            {
                Spell[SpellSlot.E].CastOnUnit(t, PacketCast);
            }
        }
        private static void Killsteal()
        {
            var t =
                HeroManager.Enemies.Where(h => h.IsValidTarget(Spell[SpellSlot.W].Range))
                    .OrderBy(h => h.Health)
                    .FirstOrDefault();

            if (!t.IsValidTarget())
                return;

            if (Helper.GetHealthPrediction.From(t, Player, SpellSlot.W, Damages.Dmg.W(t), true) &&
                Helper.GetHealthPrediction.From(t, Player, SpellSlot.W, 0f, false))
            {
                Spell[SpellSlot.W].CastIfHitchanceEquals(t, HitChance.High, PacketCast);
            }
        }
        private static void Laneclear()
        {
            var minions = MinionManager.GetMinions(Player.ServerPosition, Spell[SpellSlot.W].Range);
            var mana = Player.ManaPercent > Config.TalonConfig.Item("apollo.talon.laneclear.mana").GetValue<Slider>().Value;

            if (minions == null || !mana)
                return;

            if (Spell[SpellSlot.W].IsReady() && Helper.Use.W(Orbwalking.OrbwalkingMode.LaneClear))
            {
                var minHit = Config.TalonConfig.Item("apollo.talon.laneclear.w.hit").GetValue<Slider>().Value;
                var lineFarm =
                    MinionManager.GetBestCircularFarmLocation(
                        minions.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.W].Width * 3,
                        Spell[SpellSlot.W].Range);

                if (lineFarm.MinionsHit >= minHit)
                    Spell[SpellSlot.W].Cast(lineFarm.Position, PacketCast);
            }

            if (Spell[SpellSlot.Q].IsReady() && Helper.Use.Q(Orbwalking.OrbwalkingMode.LaneClear))
            {
                Orbwalking.BeforeAttack += args =>
                { Spell[SpellSlot.Q].Cast(PacketCast); };
            }

        }
        private static void Jungleclear()
        {
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, Spell[SpellSlot.W].Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            var mana = Player.ManaPercent > Config.TalonConfig.Item("apollo.talon.laneclear.mana").GetValue<Slider>().Value;

            if (minions == null || !mana)
                return;

            if (Spell[SpellSlot.W].IsReady() && Helper.Use.W(Orbwalking.OrbwalkingMode.LaneClear))
            {
                Spell[SpellSlot.W].Cast(minions.FirstOrDefault(), PacketCast);
            }
        }

        private static Obj_AI_Hero GetTarget(float vDefaultRange = 0,
            TargetSelector.DamageType vDefaultDamageType = TargetSelector.DamageType.Physical)
        {
            if (Math.Abs(vDefaultRange) < 0.00001)
                vDefaultRange = Spell[SpellSlot.W].Range;

            if (!Config.TalonConfig.Item("AssassinActive").GetValue<bool>())
                return TargetSelector.GetTarget(vDefaultRange, vDefaultDamageType);

            var assassinRange = Config.TalonConfig.Item("AssassinSearchRange").GetValue<Slider>().Value;

            var vEnemy =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        enemy =>
                            enemy.Team != ObjectManager.Player.Team && !enemy.IsDead && enemy.IsVisible &&
                            Config.TalonConfig.Item("Assassin" + enemy.ChampionName) != null &&
                            Config.TalonConfig.Item("Assassin" + enemy.ChampionName).GetValue<bool>() &&
                            ObjectManager.Player.Distance(enemy) < assassinRange);

            if (Config.TalonConfig.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex == 1)
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
