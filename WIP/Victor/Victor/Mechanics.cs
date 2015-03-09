using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Victor
{
    internal class Mechanics
    {
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly bool PacketCast = Config.ViktorConfig.Item("apollo.viktor.packetcast").GetValue<bool>();
        public static GameObject ChaosStorm;

        public static void Init()
        {
            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (args == null || Player.IsDead || Player.IsRecalling())
                return;
            AutoFollowR();
           
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
            var t = TargetSelector.GetTarget(1225, TargetSelector.DamageType.Magical);
            var useQ = Config.ViktorConfig.Item("apollo.viktor.combo.q.bool").GetValue<bool>();
            var useW = Config.ViktorConfig.Item("apollo.viktor.combo.w.bool").GetValue<bool>();
            var useE = Config.ViktorConfig.Item("apollo.viktor.combo.e.bool").GetValue<bool>();
            var useR = Config.ViktorConfig.Item("apollo.viktor.combo.r.bool").GetValue<bool>();
            var preE =
                (HitChance)
                    (Config.ViktorConfig.Item("apollo.viktor.combo.e.pre").GetValue<StringList>().SelectedIndex + 3);

            if (useQ)
                CastQ(t);
            if (useW)
                CastW();
            if (useE)
                CastE(t, preE);
            if (useR)
                CastR(t);
        }
        private static void Harass()
        {
            var t = TargetSelector.GetTarget(1025, TargetSelector.DamageType.Magical);
            var mana = Config.ViktorConfig.Item("apollo.viktor.harass.mana").GetValue<Slider>().Value;
            var useQ = Config.ViktorConfig.Item("apollo.viktor.harass.q.bool").GetValue<bool>();
            var useE = Config.ViktorConfig.Item("apollo.viktor.harass.e.bool").GetValue<bool>();
            var preE =
                (HitChance)
                    (Config.ViktorConfig.Item("apollo.viktor.harass.e.pre").GetValue<StringList>().SelectedIndex + 3);

            if (mana > Player.ManaPercent)
                return;

            if (useQ)
                CastQ(t);
            if (useE)
                CastE(t, preE);
        }

        private static void Laneclear()
        {
            var mana = Config.ViktorConfig.Item("apollo.viktor.laneclear.mana").GetValue<Slider>().Value;
            var useQ = Config.ViktorConfig.Item("apollo.viktor.laneclear.q.bool").GetValue<bool>();
            var useE = Config.ViktorConfig.Item("apollo.viktor.laneclear.e.bool").GetValue<bool>();
            var lastHitQ = Config.ViktorConfig.Item("apollo.viktor.laneclear.q.lasthit").GetValue<bool>();
            var lastHitCanonQ = Config.ViktorConfig.Item("apollo.viktor.laneclear.q.canon").GetValue<bool>();

            if (mana < Player.ManaPercent)
                return;
            if (useQ)
            {
                var minions = MinionManager.GetMinions(Player.ServerPosition, Spell[SpellSlot.Q].Range);

                if (minions == null)
                    return;

                var minionLasthit =
                    minions.Where(
                        h =>
                            HealthPrediction.GetHealthPrediction(
                                h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                                (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) < Damages.Dmg.Q(h) &&
                            HealthPrediction.GetHealthPrediction(
                                h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                                (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) > 0)
                        .OrderBy(h => h.Health)
                        .FirstOrDefault();
                if (lastHitQ && minionLasthit != null)
                {
                    Spell[SpellSlot.Q].CastOnUnit(minionLasthit, PacketCast);
                }

                var canonLasthit =
                    minions.Where(
                        h =>
                            h.BaseSkinName.Contains("Siege") &&
                            HealthPrediction.GetHealthPrediction(
                                h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                                (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) < Damages.Dmg.Q(h) &&
                            HealthPrediction.GetHealthPrediction(
                                h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                                (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) > 0)
                        .OrderBy(h => h.Health)
                        .FirstOrDefault();
                if (lastHitCanonQ && canonLasthit != null)
                {
                    Spell[SpellSlot.Q].CastOnUnit(canonLasthit, PacketCast);
                }

            }
            if (useE && Spell[SpellSlot.E].IsReady())
            {
                var minions = MinionManager.GetMinions(Player.ServerPosition, 1225);

                if (minions == null)
                    return;

                var minionsShort = minions.Where(h => h.IsValidTarget(Spells.ECastRange)).ToList();
                var minhit = Config.ViktorConfig.Item("apollo.viktor.laneclear.e.hit").GetValue<Slider>().Value;
                var hitListLong = new List<MinionManager.FarmLocation>();
                var hitListLongPos = new Dictionary<MinionManager.FarmLocation, Vector3>();
                var hitListShort = new List<MinionManager.FarmLocation>();
                var hitListShortPos = new Dictionary<MinionManager.FarmLocation, Vector3>();

                foreach (var minion in minionsShort)
                {
                    Spell[SpellSlot.E].UpdateSourcePosition(minion.ServerPosition, minion.ServerPosition);
                    var lineFarm =
                        MinionManager.GetBestLineFarmLocation(
                            minionsShort.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.E].Width,
                            Spell[SpellSlot.E].Range);

                    if (lineFarm.MinionsHit >= minhit)
                    {
                        hitListShort.Add(lineFarm);
                        hitListShortPos.Add(lineFarm, minion.ServerPosition);
                    }
                }
                for (var i = 0f; i < 360; i ++)
                {
                    var angleRad = Geometry.DegreeToRadian(i);
                    var direction = ObjectManager.Player.Direction.To2D().Perpendicular();
                    var rotatedPosition = (ObjectManager.Player.Position.To2D() + (Spells.ECastRange * direction.Rotated(angleRad))).To3D();
                    Spell[SpellSlot.E].UpdateSourcePosition(rotatedPosition, rotatedPosition);
                    var lineFarm =
                        MinionManager.GetBestLineFarmLocation(
                            minions.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.E].Width,
                            Spell[SpellSlot.E].Range);

                    if (lineFarm.MinionsHit >= minhit)
                    {
                        hitListLong.Add(lineFarm);
                        hitListLongPos.Add(lineFarm, rotatedPosition);
                    }
                }

                if (hitListShort.Count > 0)
                {
                    var pos1 = hitListShort.OrderBy(h => h.MinionsHit).FirstOrDefault();
                    var pos2 = hitListShortPos[pos1];

                    Spell[SpellSlot.E].Cast(pos2, pos1.Position.To3D());
                }
                else if (hitListLong.Count > 0)
                {
                    var pos1 = hitListLong.OrderBy(h => h.MinionsHit).FirstOrDefault();
                    var pos2 = hitListLongPos[pos1];

                    Spell[SpellSlot.E].Cast(pos2, pos1.Position.To3D());
                }

                hitListShort.Clear();
                hitListShortPos.Clear();
                hitListLong.Clear();
                hitListLongPos.Clear();
            }


        }
        private static void CastQ(Obj_AI_Base t)
        {
            if (!Spell[SpellSlot.Q].IsReady() || t == null)
                return;

            if (t.IsValidTarget(Spell[SpellSlot.Q].Range))
                Spell[SpellSlot.Q].CastOnUnit(t, PacketCast);
        }
        private static void CastW()
        {
            if (!Spell[SpellSlot.W].IsReady())
                return;

            var stunT =
                HeroManager.Enemies.Where(
                    h =>
                        h.IsValidTarget(Spell[SpellSlot.W].Range) &&
                        (h.HasBuffOfType(BuffType.Knockup) || h.HasBuffOfType(BuffType.Snare) ||
                         h.HasBuffOfType(BuffType.Stun) || h.HasBuffOfType(BuffType.Suppression) ||
                         h.HasBuffOfType(BuffType.Taunt)) && !h.IsInvulnerable)
                    .OrderBy(h => TargetSelector.GetPriority(h))
                    .FirstOrDefault();
            if (stunT != null && Config.ViktorConfig.Item("apollo.viktor.combo.w.stunned").GetValue<bool>())
            {
                Spell[SpellSlot.W].Cast(stunT, PacketCast, true);
            }

            var slowT =
                HeroManager.Enemies.Where(
                    h =>
                        h.IsValidTarget(Spell[SpellSlot.W].Range - Spell[SpellSlot.W].Width) &&
                        h.HasBuffOfType(BuffType.Slow)).OrderBy(h => TargetSelector.GetPriority(h)).FirstOrDefault();
            var slowTpre = Spell[SpellSlot.W].GetPrediction(slowT, true, -Spell[SpellSlot.E].Width);
            if (slowT != null && Config.ViktorConfig.Item("apollo.viktor.combo.w.slow").GetValue<bool>())
            {
                Spell[SpellSlot.W].Cast(slowTpre.CastPosition, PacketCast);
            }

            var t =
                HeroManager.Enemies.Where(h => h.IsValidTarget(300))
                    .OrderBy(h => TargetSelector.GetPriority(h))
                    .FirstOrDefault();
            var tpre = Spell[SpellSlot.E].GetPrediction(t, true);
            if (t != null)
            {
                if (tpre.Hitchance >= HitChance.High)
                {
                    Spell[SpellSlot.W].Cast(tpre.CastPosition, PacketCast);
                }
                if (tpre.AoeTargetsHitCount >=
                    Config.ViktorConfig.Item("apollo.viktor.combo.w.hit").GetValue<Slider>().Value)
                {
                    Spell[SpellSlot.W].Cast(tpre.CastPosition, PacketCast);
                }
            }

        }
        private static void CastE(Obj_AI_Base t, HitChance hit)
        {
            if (t == null)
                return;

            if (Player.Distance(t.ServerPosition) < Spells.ECastRange && Spell[SpellSlot.E].IsReady())
            {
                var sourcePosition = t.ServerPosition;
                Spell[SpellSlot.E].UpdateSourcePosition(sourcePosition, sourcePosition);
                var preE = Spell[SpellSlot.E].GetPrediction(t, true);
                if (preE.Hitchance >= hit)
                {
                    Spell[SpellSlot.E].Cast(sourcePosition, preE.CastPosition);
                }
            }
            else if (Player.Distance(t.ServerPosition) < Spells.ECastRange + Spell[SpellSlot.E].Range && Spell[SpellSlot.E].IsReady())
            {
                var sourcePosition = Player.ServerPosition.Extend(t.ServerPosition, Spells.ECastRange);
                Spell[SpellSlot.E].Speed = Spell[SpellSlot.E].Speed * 0.9f;
                Spell[SpellSlot.E].UpdateSourcePosition(sourcePosition, sourcePosition);
                var preE = Spell[SpellSlot.E].GetPrediction(t, true);
                if (preE.Hitchance >= hit)
                {
                    Spell[SpellSlot.E].Cast(sourcePosition, preE.CastPosition);
                }
            }
        }
        private static void CastR(Obj_AI_Base t)
        {
            if (t == null || !Spell[SpellSlot.R].IsReady() || ChaosStorm != null)
                return;


            var preR = Spell[SpellSlot.R].GetPrediction(t, true);
            if (t.IsValidTarget(Spell[SpellSlot.R].Range) &&
                Config.ViktorConfig.Item("apollo.viktor.combo.r.kill").GetValue<bool>() &&
                Damages.ComboDmg(t) > t.Health)
            {
                Spell[SpellSlot.R].Cast(t, PacketCast, true);
            }
            else if (preR.AoeTargetsHitCount >=
                     Config.ViktorConfig.Item("apollo.viktor.combo.r.hit").GetValue<Slider>().Value)
            {
                Spell[SpellSlot.R].Cast(preR.CastPosition, PacketCast);
            }
        }

        private static void AutoFollowR()
        {
            if (ChaosStorm != null)
            {
                var stormT = TargetSelector.GetTarget(
                    600, TargetSelector.DamageType.Magical, true, null, ChaosStorm.Position.To2D().To3D());

                Utility.DelayAction.Add(400, () => Spell[SpellSlot.R].Cast(stormT.ServerPosition));
            }
        }
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid)
                return;

            if (sender.Name.Contains("Viktor_Base_R_Droid.troy"))
            {
                ChaosStorm = sender;
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid)
                return;

            if (sender.Name.Contains("Viktor_Base_R_Droid.troy"))
            {
                ChaosStorm = null;
            }
        }
    }
}