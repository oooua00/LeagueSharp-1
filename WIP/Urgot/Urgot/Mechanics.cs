using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Urgot
{
    internal class Mechanics
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;
        private const String EBuff = "urgotcorrosivedebuff";
        public static readonly SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");

        public static void Init()
        {
            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (args == null || Player.IsDead || Player.IsRecalling())
                return;

            Killsteal();

            var activeMode = Config.Orbwalker.ActiveMode;
            if (activeMode != Orbwalking.OrbwalkingMode.Combo && activeMode != Orbwalking.OrbwalkingMode.Mixed)
                AutoQ();

            switch (activeMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                {
                    Combo();
                    break;
                }
                case Orbwalking.OrbwalkingMode.LaneClear:
                {
                    Jungleclear();
                    Laneclear();
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
            var t = TargetSelector.GetTarget(Player, Spell[SpellSlot.E].Range, TargetSelector.DamageType.Physical);
            var buffT =
                HeroManager.Enemies.Where(h => h.IsValidTarget(Spells.Q2Range) && h.HasBuff(EBuff, true))
                    .OrderBy(h => TargetSelector.GetPriority(h))
                    .FirstOrDefault();
            var useQ = Config.Urgot.Item("").GetValue<bool>();
            var useW = Config.Urgot.Item("").GetValue<bool>();
            var useE = Config.Urgot.Item("").GetValue<bool>();
            var preQ = (HitChance) (Config.Urgot.Item("").GetValue<StringList>().SelectedIndex + 3);
            var preE = (HitChance) (Config.Urgot.Item("").GetValue<StringList>().SelectedIndex + 3);

            if (IgniteSlot != SpellSlot.Unknown && IgniteSlot.IsReady() &&
                Config.Urgot.Item("apollo.urgot.combo.ignite.bool").GetValue<bool>() &&
                t.Health < Damages.ComboDmg(t))
            {
                Player.Spellbook.CastSpell(IgniteSlot, t);
            }

            if (useQ && Spell[SpellSlot.Q].IsReady())
            {
                if (t != null && t.HasBuff(EBuff, true))
                {
                    Spell[SpellSlot.Q].Cast(t);
                }
                if (((t != null && !t.HasBuff(EBuff, true)) || t == null) && buffT != null)
                {
                    Spell[SpellSlot.Q].Cast(buffT);
                }
                if (t != null)
                {
                    Spell[SpellSlot.Q].CastIfHitchanceEquals(t, preQ);
                }
            }
            if (useE && Spell[SpellSlot.E].IsReady() && t != null)
            {
                var pre = Spell[SpellSlot.E].GetPrediction(t, true);
                if (pre.Hitchance >= preE)
                    Spell[SpellSlot.E].Cast(pre.CastPosition);
            }
            if (useW && Spell[SpellSlot.W].IsReady())
            {
                var mana = Player.ManaPercent > Config.Urgot.Item("apollo.urgot.combo.w.mana").GetValue<Slider>().Value;
                if (!mana)
                    return;
                if (Config.Urgot.Item("apollo.urgot.combo.w.onq").GetValue<bool>())
                {
                    Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
                    {
                        if (sender.IsMe && args.SData.Name == Player.Spellbook.GetSpell(SpellSlot.Q).SData.Name)
                            Spell[SpellSlot.W].Cast();
                    };
                }
                else if (Config.Urgot.Item("apollo.urgot.combo.w.aa").GetValue<bool>())
                {
                    Orbwalking.BeforeAttack += args =>
                    {
                        if (args.Target.IsEnemy)
                            Spell[SpellSlot.W].Cast();
                    };
                }
            }
        }
        private static void Harass()
        {
            var t = TargetSelector.GetTarget(Player, Spells.Q2Range, TargetSelector.DamageType.Physical);
            var buffT =
                HeroManager.Enemies.Where(h => h.IsValidTarget(Spells.Q2Range) && h.HasBuff(EBuff, true))
                    .OrderBy(h => TargetSelector.GetPriority(h))
                    .FirstOrDefault();
            var useQ = Config.Urgot.Item("apollo.urgot.harass.q.bool").GetValue<bool>();
            var useE = Config.Urgot.Item("apollo.urgot.harass.e.bool").GetValue<bool>();
            var preQ = (HitChance)(Config.Urgot.Item("apollo.urgot.harass.q.pre").GetValue<StringList>().SelectedIndex + 3);
            var preE = (HitChance)(Config.Urgot.Item("apollo.urgot.harass.e.pre").GetValue<StringList>().SelectedIndex + 3);
            var mana = Player.ManaPercent > Config.Urgot.Item("apollo.urgot.harass.mana").GetValue<Slider>().Value;

            if (!mana)
                return;

            if (useQ && Spell[SpellSlot.Q].IsReady())
            {
                if (t != null && t.HasBuff(EBuff, true))
                {
                    Spell[SpellSlot.Q].Cast(t);
                }
                if (((t != null && !t.HasBuff(EBuff, true)) || t == null) && buffT != null)
                {
                    Spell[SpellSlot.Q].Cast(buffT);
                }
                if (t != null)
                {
                    Spell[SpellSlot.Q].CastIfHitchanceEquals(t, preQ);
                }
            }
            if (useE && Spell[SpellSlot.E].IsReady() && t != null)
            {
                var pre = Spell[SpellSlot.E].GetPrediction(t, true);
                if (pre.Hitchance >= preE)
                    Spell[SpellSlot.E].Cast(pre.CastPosition);
            }
        }
        private static void Jungleclear()
        {
            var minions = MinionManager.GetMinions(
                Player.ServerPosition, Spells.Q2Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var useQ = Config.Urgot.Item("apollo.urgot.laneclear.q.bool").GetValue<bool>();
            var useE = Config.Urgot.Item("apollo.urgot.laneclear.e.bool").GetValue<bool>();
            var mana = Player.ManaPercent > Config.Urgot.Item("apollo.urgot.laneclear.mana").GetValue<Slider>().Value;

            if (minions == null || !mana)
                return;

            if (useE && Spell[SpellSlot.E].IsReady())
            {
                var minionE =
                    minions.Where(h => h.IsValidTarget(Spell[SpellSlot.E].Range))
                        .OrderBy(h => h.MaxHealth)
                        .FirstOrDefault();

                if (minionE != null)
                    Spell[SpellSlot.E].Cast(minionE, true);
            }

            if (useQ && Spell[SpellSlot.Q].IsReady())
            {
                var minionbuffQ = minions.Where(h => h.HasBuff(EBuff, true)).OrderBy(h => h.MaxHealth).FirstOrDefault();
                var minionQ =
                    minions.Where(
                        h =>
                            h.IsValidTarget(Spell[SpellSlot.Q].Range) &&
                            Spell[SpellSlot.Q].GetPrediction(h).Hitchance != HitChance.Collision)
                        .OrderBy(h => h.Health)
                        .FirstOrDefault();
                if (minionbuffQ != null)
                    Spell[SpellSlot.Q].Cast(minionbuffQ.ServerPosition);
                else if (minionQ != null)
                    Spell[SpellSlot.Q].Cast(minionQ);
            }
        }
        private static void Laneclear()
        {
            var minions = MinionManager.GetMinions(Player.ServerPosition, Spells.Q2Range);
            var useQ = Config.Urgot.Item("apollo.urgot.laneclear.q.bool").GetValue<bool>();
            var useE = Config.Urgot.Item("apollo.urgot.laneclear.e.bool").GetValue<bool>();
            var hitE = Config.Urgot.Item("apollo.urgot.laneclear.e.hit").GetValue<Slider>().Value;
            var lasthitQ = Config.Urgot.Item("apollo.urgot.laneclear.q.lasthit").GetValue<bool>();
            var mana = Player.ManaPercent > Config.Urgot.Item("apollo.urgot.laneclear.mana").GetValue<Slider>().Value;

            if (minions == null ||  !mana)
                return;

            if (useQ && Spell[SpellSlot.Q].IsReady())
            {
                if (lasthitQ)
                {
                    var minionQ =
                        minions.Where(
                            h =>
                                h.IsValidTarget(Spell[SpellSlot.Q].Range) &&
                                HealthPrediction.GetHealthPrediction(
                                    h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                                    (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) <
                                Player.GetSpellDamage(h, SpellSlot.Q) &&
                                HealthPrediction.GetHealthPrediction(
                                    h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                                    (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) > 0 &&
                                Spell[SpellSlot.Q].GetPrediction(h).Hitchance != HitChance.Collision)
                            .OrderBy(h => h.Health)
                            .FirstOrDefault();
                    if (minionQ != null)
                        Spell[SpellSlot.Q].Cast(minionQ);
                }
                var minionbuffQ =
                    minions.Where(
                        h =>
                            h.HasBuff(EBuff, true) &&
                            HealthPrediction.GetHealthPrediction(
                                h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                                (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) <
                            Player.GetSpellDamage(h, SpellSlot.Q) &&
                            HealthPrediction.GetHealthPrediction(
                                h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                                (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) > 0)
                        .OrderBy(h => h.Health)
                        .FirstOrDefault();
                if (minionbuffQ != null)
                {
                    Spell[SpellSlot.Q].Cast(minionbuffQ.ServerPosition);
                }
            }
            if (useE && Spell[SpellSlot.E].IsReady())
            {
                var minionsE = minions.Where(h => h.IsValidTarget(Spell[SpellSlot.E].Range)).OrderBy(h => h.Health);
                var farmlocationE =
                    MinionManager.GetBestCircularFarmLocation(
                        minionsE.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.E].Width,
                        Spell[SpellSlot.E].Range);

                if (farmlocationE.MinionsHit >= hitE)
                    Spell[SpellSlot.E].Cast(farmlocationE.Position);
            }
        }
        private static void AutoQ()
        {
            var t =
                HeroManager.Enemies.Where(h => h.IsValidTarget(Spells.Q2Range) && h.HasBuff(EBuff, true))
                    .OrderBy(h => TargetSelector.GetPriority(h))
                    .FirstOrDefault();

            if (t == null || !Spell[SpellSlot.Q].IsReady() ||
                !Config.Urgot.Item("apollo.urgot.misc.autoq").GetValue<bool>())
                return;

            Spell[SpellSlot.Q].Cast(t);
        }
        private static void Killsteal()
        {
            var t =
                HeroManager.Enemies.Where(
                    h =>
                        h.IsValidTarget(Spell[SpellSlot.Q].Range) &&
                        HealthPrediction.GetHealthPrediction(
                            h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                            (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) <
                        Player.GetSpellDamage(h, SpellSlot.Q) &&
                        HealthPrediction.GetHealthPrediction(
                            h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                            (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) > 0)
                    .OrderBy(h => h.Health)
                    .FirstOrDefault();

            if (t == null || Config.Urgot.Item("apollo.urgot.ks.q.bool").GetValue<bool>() || !Spell[SpellSlot.Q].IsReady())
                return;

            Spell[SpellSlot.Q].CastIfHitchanceEquals(t, HitChance.High);
        }
        private static void OnInterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var useR = Config.Urgot.Item("apollo.urgot.interrupter.r.bool").GetValue<bool>();
            var dangerLvl =
                (Interrupter2.DangerLevel)
                    (Config.Urgot.Item("apollo.urgot.interrupter.dangerlvl").GetValue<StringList>().SelectedIndex + 2);

            if (args == null || args.DangerLevel < dangerLvl)
                return;

            if (useR && Spell[SpellSlot.R].IsReady() && sender.IsValidTarget(Spell[SpellSlot.R].Range))
                Spell[SpellSlot.R].CastOnUnit(sender);
        }
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Type != GameObjectType.obj_SpellMissile)
                return;

            var spell = (Obj_SpellMissile) sender;
            var caster = spell.SpellCaster;

            if (sender.IsValid && Config.Urgot.Item("apollo.urgot.misc.autor").GetValue<bool>() &&
                Spell[SpellSlot.R].IsReady() && caster.Type == GameObjectType.obj_AI_Turret &&
                caster.Position.Distance(Player.ServerPosition) < 750 && caster.IsAlly && spell.Target.IsEnemy &&
                spell.Target.Type == GameObjectType.obj_AI_Hero)
            {
                var t = spell.Target as Obj_AI_Base;
                if (t != null && t.IsValidTarget(Spell[SpellSlot.R].Range) && !t.IsInvulnerable)
                    Spell[SpellSlot.R].CastOnUnit(t);
            }
            if (sender.IsValid && Config.Urgot.Item("apollo.urgot.misc.autow").GetValue<bool>() &&
                Spell[SpellSlot.W].IsReady()) {}
            if (caster.IsEnemy)
            {
                var shield = new [] { 60, 100, 140, 180, 220 }[Spell[SpellSlot.W].Level] +
                             .8 * Player.TotalMagicalDamage() + .08 * Player.MaxMana;

                if (spell.SData.Name.Contains("BasicAttack"))
                {
                    if (spell.Target.IsMe && Player.Health <= caster.GetAutoAttackDamage(Player, true) &&
                        Player.Health + shield > caster.GetAutoAttackDamage(Player, true))
                        Spell[SpellSlot.W].Cast();
                }
                else if (spell.Target.IsMe || spell.EndPosition.Distance(Player.Position) <= 130)
                {
                    if (spell.SData.Name == "summonerdot")
                    {
                        if (Player.Health <=
                            (caster as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite) &&
                            Player.Health + shield >
                            (caster as Obj_AI_Hero).GetSummonerSpellDamage(Player, Damage.SummonerSpell.Ignite))
                            Spell[SpellSlot.W].Cast();
                    }
                    else if (Player.Health <=
                             (caster as Obj_AI_Hero).GetSpellDamage(
                                 Player, (caster as Obj_AI_Hero).GetSpellSlot(spell.SData.Name), 1) &&
                             Player.Health + shield >
                             (caster as Obj_AI_Hero).GetSpellDamage(
                                 Player, (caster as Obj_AI_Hero).GetSpellSlot(spell.SData.Name), 1))
                        Spell[SpellSlot.W].Cast();
                }
            }
        }
    }
}
