// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable InvertIf
// ReSharper disable FunctionComplexityOverflow
// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Viktor
{
    internal static class Mechanics
    {
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly bool PacketCast = Config.ViktorConfig.Item("apollo.viktor.packetcast").GetValue<bool>();
        private static GameObject _chaosStorm;
        public static readonly SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");

        public static void Init()
        {
            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (args == null || Player.IsDead || Player.IsRecalling())
            {
                return;
            }
            AutoFollowR();
            KillSteal();

            KeyBind key = Config.ViktorConfig.Item("apollo.viktor.harass.key").GetValue<KeyBind>();
            if (key.Active)
            {
                Harass();
            }
            //Notifications.AddNotification("AutoHarass: " + key.Active.ToString(), 1, false);
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
            Obj_AI_Hero t = TargetSelector.GetTarget(
                Spell[SpellSlot.E].Range + Spells.ECastRange, TargetSelector.DamageType.Magical);
            bool useQ = Config.ViktorConfig.Item("apollo.viktor.combo.q.bool").GetValue<bool>();
            bool useW = Config.ViktorConfig.Item("apollo.viktor.combo.w.bool").GetValue<bool>();
            bool useE = Config.ViktorConfig.Item("apollo.viktor.combo.e.bool").GetValue<bool>();
            bool useR = Config.ViktorConfig.Item("apollo.viktor.combo.r.bool").GetValue<bool>();
            HitChance preE =
                (HitChance)
                    (Config.ViktorConfig.Item("apollo.viktor.combo.e.pre").GetValue<StringList>().SelectedIndex + 3);

            if (IgniteSlot != SpellSlot.Unknown && IgniteSlot.IsReady() &&
                Config.ViktorConfig.Item("apollo.viktor.combo.ignite.bool").GetValue<bool>() &&
                t.Health < Damages.ComboDmg(t))
            {
                Player.Spellbook.CastSpell(IgniteSlot, t);
            }

            if (useQ)
            {
                CastQ(t);
            }
            if (useW)
            {
                CastW();
            }
            if (useE)
            {
                CastE(t, preE);
            }
            if (useR)
            {
                CastR(t);
            }
        }

        private static void Harass()
        {
            Obj_AI_Hero t = TargetSelector.GetTarget(1025, TargetSelector.DamageType.Magical);
            int mana = Config.ViktorConfig.Item("apollo.viktor.harass.mana").GetValue<Slider>().Value;
            bool useQ = Config.ViktorConfig.Item("apollo.viktor.harass.q.bool").GetValue<bool>();
            bool useE = Config.ViktorConfig.Item("apollo.viktor.harass.e.bool").GetValue<bool>();
            HitChance preE =
                (HitChance)
                    (Config.ViktorConfig.Item("apollo.viktor.harass.e.pre").GetValue<StringList>().SelectedIndex + 3);

            if (mana > Player.ManaPercent)
            {
                return;
            }

            if (useQ)
            {
                CastQ(t);
            }
            if (useE)
            {
                CastE(t, preE);
            }
        }

        private static void Laneclear()
        {
            int mana = Config.ViktorConfig.Item("apollo.viktor.laneclear.mana").GetValue<Slider>().Value;
            bool useQ = Config.ViktorConfig.Item("apollo.viktor.laneclear.q.bool").GetValue<bool>();
            bool useE = Config.ViktorConfig.Item("apollo.viktor.laneclear.e.bool").GetValue<bool>();
            bool lastHitQ = Config.ViktorConfig.Item("apollo.viktor.laneclear.q.lasthit").GetValue<bool>();
            bool lastHitCanonQ = Config.ViktorConfig.Item("apollo.viktor.laneclear.q.canon").GetValue<bool>();
            bool toasterProofE = Config.ViktorConfig.Item("apollo.viktor.laneclear.e.ToasterProofE").GetValue<bool>();

            if (mana < Player.ManaPercent)
            {
                return;
            }
            if (useQ)
            {
                List<Obj_AI_Base> minions = MinionManager.GetMinions(Player.ServerPosition, Spell[SpellSlot.Q].Range);

                if (minions == null)
                {
                    return;
                }

                Obj_AI_Base minionLasthit =
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

                Obj_AI_Base canonLasthit =
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
                List<Obj_AI_Base> minions = MinionManager.GetMinions(
                    Player.ServerPosition, Spell[SpellSlot.E].Range + Spells.ECastRange);

                if (minions == null)
                {
                    return;
                }

                List<Obj_AI_Base> minionsShort = minions.Where(h => h.IsValidTarget(Spells.ECastRange)).ToList();
                int minhit = Config.ViktorConfig.Item("apollo.viktor.laneclear.e.hit").GetValue<Slider>().Value;
                List<MinionManager.FarmLocation> hitListLong = new List<MinionManager.FarmLocation>();
                Dictionary<MinionManager.FarmLocation, Vector3> hitListLongPos =
                    new Dictionary<MinionManager.FarmLocation, Vector3>();
                List<MinionManager.FarmLocation> hitListShort = new List<MinionManager.FarmLocation>();
                Dictionary<MinionManager.FarmLocation, Vector3> hitListShortPos =
                    new Dictionary<MinionManager.FarmLocation, Vector3>();

                foreach (Obj_AI_Base minion in minionsShort)
                {
                    Spell[SpellSlot.E].UpdateSourcePosition(minion.ServerPosition, minion.ServerPosition);
                    MinionManager.FarmLocation lineFarm =
                        MinionManager.GetBestLineFarmLocation(
                            minionsShort.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.E].Width,
                            Spell[SpellSlot.E].Range);

                    if (lineFarm.MinionsHit >= minhit)
                    {
                        hitListShort.Add(lineFarm);
                        hitListShortPos.Add(lineFarm, minion.ServerPosition);
                    }
                }


                if (toasterProofE)
                {
                    bool ex = false;
                    Vector2 playDirFace = Player.Direction.To2D();
                    //gen raw vectors to create cone
                    Vector2[] rawCone = {
                        playDirFace, new Vector2(playDirFace.X - 425, playDirFace.Y + 1275),
                        new Vector2(playDirFace.X + 425, playDirFace.Y + 1275)
                    };
                    //convert raw cone to points (I just use this to keep my code readable could just as well use Vector2.X and Vector2.Y)
                    Point a = new Point((int) rawCone[1].X, (int) rawCone[1].Y);
                    Point b = new Point((int) rawCone[0].X, (int) rawCone[0].Y);
                    Point c = new Point((int) rawCone[2].X, (int) rawCone[2].Y);
                    //using dot product to find the angle, dot product: math is fun boyz
                    //a.b = (|ba|*|bc|)*cos(α)
                    //a.b = ax*bx+ay*by = |ba|*|bc|*cos(α)
                    //α = acos((ax*bx+ay*by)/(|ba|*|bc|))
                    double rangle =
                        Math.Acos(
                            ((a.X * c.X) + (a.Y * b.Y)) /
                            (Vector2.Distance(rawCone[0], rawCone[1]) * Vector2.Distance(rawCone[0], rawCone[2])));
                    //get starting angle
                    Vector2 dv = new Vector2(rawCone[1].X, rawCone[0].Y);
                    Point d = new Point((int) rawCone[1].X, (int) rawCone[0].Y);
                    //d = new a, a = new c, b unchanged duuuh rolf
                    double sangle =
                        Math.Acos(
                            ((d.X * a.X) + (d.Y * b.Y)) /
                            (Vector2.Distance(rawCone[0], dv) * Vector2.Distance(rawCone[0], rawCone[1])));

                    //executerino
                    for (float i = (float) sangle; i < rangle; i++)
                    {
                        float angleRad = Geometry.DegreeToRadian(i);
                        Vector2 direction = ObjectManager.Player.Direction.To2D().Perpendicular();
                        Vector3 rotatedPosition =
                            (ObjectManager.Player.Position.To2D() + (Spells.ECastRange * direction.Rotated(angleRad)))
                                .To3D();
                        Spell[SpellSlot.E].UpdateSourcePosition(rotatedPosition, rotatedPosition);
                        MinionManager.FarmLocation lineFarm =
                            MinionManager.GetBestLineFarmLocation(
                                minions.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.E].Width,
                                Spell[SpellSlot.E].Range);

                        if (lineFarm.MinionsHit >= minhit)
                        {
                            ex = true;
                            hitListLong.Add(lineFarm);
                            hitListLongPos.Add(lineFarm, rotatedPosition);
                        }
                    }
                    if (!ex)
                    {
                        double rangle2 = (360 - (rangle * 2)) / 2;
                        //2.1 check left sector
                        for (float i = (float)sangle; i < rangle2; i--)
                        {
                            float angleRad = Geometry.DegreeToRadian(i);
                            Vector2 direction = ObjectManager.Player.Direction.To2D().Perpendicular();
                            Vector3 rotatedPosition =
                                (ObjectManager.Player.Position.To2D() + (Spells.ECastRange * direction.Rotated(angleRad)))
                                    .To3D();
                            Spell[SpellSlot.E].UpdateSourcePosition(rotatedPosition, rotatedPosition);
                            MinionManager.FarmLocation lineFarm =
                                MinionManager.GetBestLineFarmLocation(
                                    minions.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.E].Width,
                                    Spell[SpellSlot.E].Range);

                            if (lineFarm.MinionsHit >= minhit)
                            {
                                ex = true;
                                hitListLong.Add(lineFarm);
                                hitListLongPos.Add(lineFarm, rotatedPosition);
                            }
                        }
                        if (!ex)
                        {
                            //3 check right sector
                            for (float i = (float)sangle + (float)rangle; i < rangle2; i++)
                            {
                                float angleRad = Geometry.DegreeToRadian(i);
                                Vector2 direction = ObjectManager.Player.Direction.To2D().Perpendicular();
                                Vector3 rotatedPosition =
                                    (ObjectManager.Player.Position.To2D() + (Spells.ECastRange * direction.Rotated(angleRad)))
                                        .To3D();
                                Spell[SpellSlot.E].UpdateSourcePosition(rotatedPosition, rotatedPosition);
                                MinionManager.FarmLocation lineFarm =
                                    MinionManager.GetBestLineFarmLocation(
                                        minions.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.E].Width,
                                        Spell[SpellSlot.E].Range);

                                if (lineFarm.MinionsHit >= minhit)
                                {
                                    hitListLong.Add(lineFarm);
                                    hitListLongPos.Add(lineFarm, rotatedPosition);
                                }
                            }
                            if (!ex)
                            {
                                //4 check lower sector
                                for (float i = (float)sangle + (float)rangle + (float)rangle2; i < rangle2; i++)
                                {
                                    float angleRad = Geometry.DegreeToRadian(i);
                                    Vector2 direction = ObjectManager.Player.Direction.To2D().Perpendicular();
                                    Vector3 rotatedPosition =
                                        (ObjectManager.Player.Position.To2D() + (Spells.ECastRange * direction.Rotated(angleRad)))
                                            .To3D();
                                    Spell[SpellSlot.E].UpdateSourcePosition(rotatedPosition, rotatedPosition);
                                    MinionManager.FarmLocation lineFarm =
                                        MinionManager.GetBestLineFarmLocation(
                                            minions.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.E].Width,
                                            Spell[SpellSlot.E].Range);

                                    if (lineFarm.MinionsHit >= minhit)
                                    {
                                        hitListLong.Add(lineFarm);
                                        hitListLongPos.Add(lineFarm, rotatedPosition);
                                    }
                                }
                            }
                        }
                    }
                    
                }
                else
                {
                    for (float i = 0f; i < 360; i++)
                    {
                        float angleRad = Geometry.DegreeToRadian(i);
                        Vector2 direction = ObjectManager.Player.Direction.To2D().Perpendicular();
                        Vector3 rotatedPosition =
                            (ObjectManager.Player.Position.To2D() + (Spells.ECastRange * direction.Rotated(angleRad))).To3D(
                                );
                        Spell[SpellSlot.E].UpdateSourcePosition(rotatedPosition, rotatedPosition);
                        MinionManager.FarmLocation lineFarm =
                            MinionManager.GetBestLineFarmLocation(
                                minions.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.E].Width,
                                Spell[SpellSlot.E].Range);

                        if (lineFarm.MinionsHit >= minhit)
                        {
                            hitListLong.Add(lineFarm);
                            hitListLongPos.Add(lineFarm, rotatedPosition);
                        }
                    }
                }

                if (hitListShort.Count > 0 &&
                    hitListShort.OrderBy(h => h.MinionsHit).FirstOrDefault().MinionsHit >=
                    hitListLong.OrderBy(h => h.MinionsHit).FirstOrDefault().MinionsHit)
                {
                    MinionManager.FarmLocation pos1 = hitListShort.OrderBy(h => h.MinionsHit).FirstOrDefault();
                    Vector3 pos2 = hitListShortPos[pos1];

                    Spell[SpellSlot.E].Cast(pos2, pos1.Position.To3D());
                }
                else if (hitListLong.Count > 0)
                {
                    MinionManager.FarmLocation pos1 = hitListLong.OrderBy(h => h.MinionsHit).FirstOrDefault();
                    Vector3 pos2 = hitListLongPos[pos1];

                    Spell[SpellSlot.E].Cast(pos2, pos1.Position.To3D());
                }

                hitListShort.Clear();
                hitListShortPos.Clear();
                hitListLong.Clear();
                hitListLongPos.Clear();
            }
        }

        private static void Jungleclear()
        {
            List<Obj_AI_Base> minions = MinionManager.GetMinions(
                Player.ServerPosition, Spell[SpellSlot.E].Range + Spells.ECastRange, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            IOrderedEnumerable<Obj_AI_Base> minionsQ =
                minions.Where(h => h.IsValidTarget(Spell[SpellSlot.Q].Range)).OrderBy(h => h.MaxHealth);
            int mana = Config.ViktorConfig.Item("apollo.viktor.laneclear.mana").GetValue<Slider>().Value;

            if (minions == null || mana > Player.ManaPercent)
            {
                return;
            }

            bool useQ = Config.ViktorConfig.Item("apollo.viktor.laneclear.q.bool").GetValue<bool>();
            bool useE = Config.ViktorConfig.Item("apollo.viktor.laneclear.e.bool").GetValue<bool>();

            if (useQ && Spell[SpellSlot.Q].IsReady() && minionsQ != null)
            {
                Spell[SpellSlot.Q].CastOnUnit(minionsQ.FirstOrDefault(), PacketCast);
            }

            if (useE && Spell[SpellSlot.E].IsReady())
            {
                if (Player.Distance(minions.FirstOrDefault()) < Spells.ECastRange)
                {
                    Obj_AI_Base objAiBase = minions.FirstOrDefault();
                    if (objAiBase != null)
                    {
                        Vector3 sourcePosition = objAiBase.ServerPosition;
                        Spell[SpellSlot.E].UpdateSourcePosition(sourcePosition, sourcePosition);
                        MinionManager.FarmLocation lineFarm =
                            MinionManager.GetBestLineFarmLocation(
                                minions.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.E].Width,
                                Spell[SpellSlot.E].Range);
                        Spell[SpellSlot.E].Cast(sourcePosition, lineFarm.Position.To3D());
                    }
                }
                else
                {
                    Obj_AI_Base objAiBase = minions.FirstOrDefault();
                    if (objAiBase != null)
                    {
                        Vector3 sourcePosition = Player.ServerPosition.Extend(
                            objAiBase.ServerPosition, Spells.ECastRange);
                        Spell[SpellSlot.E].UpdateSourcePosition(sourcePosition, sourcePosition);
                        MinionManager.FarmLocation lineFarm =
                            MinionManager.GetBestLineFarmLocation(
                                minions.Select(m => m.ServerPosition.To2D()).ToList(), Spell[SpellSlot.E].Width,
                                Spell[SpellSlot.E].Range);
                        Spell[SpellSlot.E].Cast(sourcePosition, lineFarm.Position.To3D());
                    }
                }
            }
        }

        private static void CastQ(Obj_AI_Base t)
        {
            if (!Spell[SpellSlot.Q].IsReady() || t == null || !Spell[SpellSlot.Q].IsInRange(t))
            {
                return;
            }
            if (Orbwalking.InAutoAttackRange(t))
            {
                Orbwalking.BeforeAttack += eventArgs => { Spell[SpellSlot.Q].CastOnUnit(t, PacketCast); };
            }
            else if (!Config.ViktorConfig.Item("apollo.viktor.combo.q.dont").GetValue<bool>())
            {
                Spell[SpellSlot.Q].CastOnUnit(t, PacketCast);
            }
        }

        private static void CastW()
        {
            if (!Spell[SpellSlot.W].IsReady())
            {
                return;
            }

            Obj_AI_Hero stunT =
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

            Obj_AI_Hero slowT =
                HeroManager.Enemies.Where(
                    h =>
                        h.IsValidTarget(Spell[SpellSlot.W].Range - Spell[SpellSlot.W].Width) &&
                        h.HasBuffOfType(BuffType.Slow)).OrderBy(h => TargetSelector.GetPriority(h)).FirstOrDefault();
            PredictionOutput slowTpre = Spell[SpellSlot.W].GetPrediction(slowT, true, -Spell[SpellSlot.E].Width);
            if (slowT != null && Config.ViktorConfig.Item("apollo.viktor.combo.w.slow").GetValue<bool>())
            {
                Spell[SpellSlot.W].Cast(slowTpre.CastPosition, PacketCast);
            }

            Obj_AI_Hero t =
                HeroManager.Enemies.Where(h => h.IsValidTarget(300))
                    .OrderBy(h => TargetSelector.GetPriority(h))
                    .FirstOrDefault();
            PredictionOutput tpre = Spell[SpellSlot.E].GetPrediction(t, true);
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
            {
                return;
            }

            if (Player.Distance(t.ServerPosition) < Spells.ECastRange && Spell[SpellSlot.E].IsReady())
            {
                Vector3 sourcePosition = t.ServerPosition;
                Spell[SpellSlot.E].UpdateSourcePosition(sourcePosition, sourcePosition);
                PredictionOutput preE = Spell[SpellSlot.E].GetPrediction(t, true);
                if (preE.Hitchance >= hit)
                {
                    Spell[SpellSlot.E].Cast(sourcePosition, preE.CastPosition);
                }
            }
            else if (Player.Distance(t.ServerPosition) < Spells.ECastRange + Spell[SpellSlot.E].Range &&
                     Spell[SpellSlot.E].IsReady())
            {
                Vector3 sourcePosition = Player.ServerPosition.Extend(t.ServerPosition, Spells.ECastRange);
                Spell[SpellSlot.E].UpdateSourcePosition(sourcePosition, sourcePosition);
                PredictionOutput preE = Spell[SpellSlot.E].GetPrediction(t, true);
                if (preE.Hitchance >= hit)
                {
                    Spell[SpellSlot.E].Cast(sourcePosition, preE.CastPosition);
                }
            }
        }

        private static void CastR(Obj_AI_Base t)
        {
            if (t == null || !Spell[SpellSlot.R].IsReady() || _chaosStorm != null)
            {
                return;
            }


            PredictionOutput preR = Spell[SpellSlot.R].GetPrediction(t, true);
            if (t.IsValidTarget(Spell[SpellSlot.R].Range) &&
                Config.ViktorConfig.Item("apollo.viktor.combo.r.kill").GetValue<bool>() &&
                Damages.ComboDmg(t) > t.Health &&
                t.HealthPercent > Config.ViktorConfig.Item("apollo.viktor.combo.r.minhp").GetValue<Slider>().Value)
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
            if (_chaosStorm != null)
            {
                Obj_AI_Hero stormT = TargetSelector.GetTarget(
                    600, TargetSelector.DamageType.Magical, true, null, _chaosStorm.Position.To2D().To3D());

                Utility.DelayAction.Add(400, () => Spell[SpellSlot.R].Cast(stormT.ServerPosition));
            }
        }

        private static void KillSteal()
        {
            bool useE = Config.ViktorConfig.Item("apollo.viktor.ks.e.bool").GetValue<bool>();
            Obj_AI_Hero t =
                HeroManager.Enemies.Where(
                    h =>
                        h.IsValidTarget(Spell[SpellSlot.E].Range + Spells.ECastRange) &&
                        HealthPrediction.GetHealthPrediction(
                            h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                            (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) < Damages.Dmg.Q(h) &&
                        HealthPrediction.GetHealthPrediction(
                            h, (int) (Player.Distance(h) / Spell[SpellSlot.Q].Speed),
                            (int) (Spell[SpellSlot.Q].Delay * 1000 + Game.Ping / 2f)) > 0)
                    .OrderBy(h => h.Health)
                    .FirstOrDefault();
            if (t == null)
            {
                return;
            }
            if (useE && Spell[SpellSlot.E].IsReady())
            {
                if (Player.Distance(t) < Spells.ECastRange)
                {
                    Vector3 sourcePosition = t.ServerPosition;
                    Spell[SpellSlot.E].UpdateSourcePosition(sourcePosition, sourcePosition);
                    PredictionOutput preE = Spell[SpellSlot.E].GetPrediction(t, true);

                    Spell[SpellSlot.E].Cast(sourcePosition, preE.CastPosition);
                }
                else
                {
                    Vector3 sourcePosition = Player.ServerPosition.Extend(t.ServerPosition, Spells.ECastRange);
                    Spell[SpellSlot.E].UpdateSourcePosition(sourcePosition, sourcePosition);
                    PredictionOutput preE = Spell[SpellSlot.E].GetPrediction(t, true);

                    Spell[SpellSlot.E].Cast(sourcePosition, preE.CastPosition);
                }
            }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.ViktorConfig.Item("apollo.viktor.gapcloser.w.bool").GetValue<bool>() &&
                Player.Distance(gapcloser.Sender) < Orbwalking.GetRealAutoAttackRange(Player) &&
                Spell[SpellSlot.W].IsReady())
            {
                Spell[SpellSlot.W].Cast(gapcloser.End, PacketCast);
            }
        }

        private static void OnInterruptableTarget(Obj_AI_Hero unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                bool useW = Config.ViktorConfig.Item("apollo.viktor.interrupt.w.bool").GetValue<bool>();
                bool useR = Config.ViktorConfig.Item("apollo.viktor.interrupt.r.bool").GetValue<bool>();

                if (useW && Spell[SpellSlot.W].IsReady() && unit.IsValidTarget(Spell[SpellSlot.W].Range) &&
                    (Game.Time + 1.5 + Spell[SpellSlot.W].Delay) >= args.EndTime)
                {
                    Spell[SpellSlot.W].Cast(unit.ServerPosition, PacketCast);
                }
                else if (useR && unit.IsValidTarget(Spell[SpellSlot.R].Range))
                {
                    Spell[SpellSlot.R].Cast(unit.ServerPosition, PacketCast);
                }
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid)
            {
                return;
            }

            if (sender.Name.Contains("Viktor_Base_R_Droid.troy"))
            {
                _chaosStorm = sender;
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid)
            {
                return;
            }

            if (sender.Name.Contains("Viktor_Base_R_Droid.troy"))
            {
                _chaosStorm = null;
            }
        }
    }
}