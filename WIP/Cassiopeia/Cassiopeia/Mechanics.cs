using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Cassiopeia
{
    internal class Mechanics
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static bool PacketCast;
        private static readonly Dictionary<SpellSlot, Spell> Spells = SpellClass.Spells;
        public static class Last
        {
            public static int Q { get; set; }
            public static int W { get; set; }
            public static int E { get; set; }
        }

        public static void Init()
        {
            Game.OnGameUpdate += OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            switch (Configs.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                {
                    Combo();
                    break;
                }
                case Orbwalking.OrbwalkingMode.LaneClear:
                {
                    LaneClear();
                    break;
                }
                case Orbwalking.OrbwalkingMode.Mixed:
                {
                    Harass();
                    break;
                }
            }

            PacketCast = Configs.CassioConfig.Item("apollo.cassio.packetcast").GetValue<bool>();
        }

        private static void Combo()
        {
            var t = TargetSelector.GetTarget(Spells[SpellSlot.Q].Range, TargetSelector.DamageType.Magical);
            if (t == null)
                return;

            Configs.Orbwalker.SetAttack(!Configs.CassioConfig.Item("apollo.cassio.misc.orb").GetValue<bool>()); //todo check

            CastUlti(t);

            if (Instances.UseSpell(SpellSlot.Q) && Environment.TickCount > Last.Q + Instances.Delay(SpellSlot.Q))
            {
                Spells[SpellSlot.Q].CastIfHitchanceEquals(t, Instances.HitChance(SpellSlot.Q), PacketCast);
            }


            if (Instances.UseSpell(SpellSlot.W) && Environment.TickCount > Last.W + Instances.Delay(SpellSlot.W))
            {
                Spells[SpellSlot.W].CastIfHitchanceEquals(t, Instances.HitChance(SpellSlot.W), PacketCast);
            }

            var buffEndTime = Instances.GetPoisonBuffEndTime(t);
            if (Instances.UseSpell(SpellSlot.E) && Environment.TickCount > Last.E + Instances.Delay(SpellSlot.E) && buffEndTime > (Game.Time + Spells[SpellSlot.E].Delay))
            {
                if (t.HasBuffOfType(BuffType.Poison))
                {
                    Spells[SpellSlot.E].Cast(t, PacketCast);
                }
                if (Player.GetSpellDamage(t, SpellSlot.Q) > t.Health && !Spells[SpellSlot.Q].IsReady() &&
                    !Spells[SpellSlot.W].IsReady() && Configs.CassioConfig.Item("apollo.cassio.e.kill").GetValue<bool>())
                {
                    Spells[SpellSlot.E].Cast(t, PacketCast);
                }
                else
                {
                    var tar =
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(
                                h =>
                                    h.IsEnemy && h.IsValidTarget(Spells[SpellSlot.E].Range) &&
                                    h.HasBuffOfType(BuffType.Poison))
                            .OrderBy(h => TargetSelector.GetPriority(h));

                    Spells[SpellSlot.E].Cast(tar.First(), PacketCast);
                }
            }
        }

        private static void CastUlti(Obj_AI_Hero t)
        {
            if (!Spells[SpellSlot.R].IsReady())
                return;

            var selTar = TargetSelector.SelectedTarget;

            if (selTar != null && !selTar.IsInvulnerable && Spells[SpellSlot.R].WillHit(selTar, Player.ServerPosition) &&
                Configs.CassioConfig.Item("apollo.cassio.r.seltar").GetValue<bool>())
            {
                Instances.CastR(Player.IsFacing(selTar), selTar.ServerPosition);
            }
            else if (!t.IsInvulnerable && Spells[SpellSlot.R].WillHit(t, Player.ServerPosition) &&
                     Configs.CassioConfig.Item("apollo.cassio.r.kill").GetValue<bool>())
            {
                if (Damages.ComboDmg(t) > t.Health)
                    Instances.CastR(Player.IsFacing(t), t.ServerPosition);
            }

            var castPred = Spells[SpellSlot.R].GetPrediction(t, true, Spells[SpellSlot.R].Range);
            var enemiesHit =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(h => Spells[SpellSlot.R].WillHit(h, castPred.CastPosition) && !h.IsInvulnerable)
                    .ToList();
            var enemiesFacing = enemiesHit.Where(h => h.IsFacing(Player)).ToList();
            var enemiesNotFacing = enemiesHit.Where(h => !h.IsFacing(Player)).ToList();
            var minStun = Configs.CassioConfig.Item("apollo.cassio.r.minhit.stunned").GetValue<Slider>().Value;
            var minHit = Configs.CassioConfig.Item("apollo.cassio.r.minhit.hit").GetValue<Slider>().Value;

            switch (Configs.CassioConfig.Item("apollo.cassio.r.minhit.mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                {
                    if (enemiesHit.Count() >= minHit)
                        SpellClass.Spells[SpellSlot.Q].Cast(castPred.CastPosition, PacketCast);
                            break;
                }
                case 1:
                {
                    if (enemiesFacing.Count() >= minStun && enemiesNotFacing.Count < minStun)
                        SpellClass.Spells[SpellSlot.Q].Cast(castPred.CastPosition, PacketCast);

                    else if (enemiesNotFacing.Count >= minStun)
                        Instances.CastR(false, castPred.CastPosition);
                    break;
                }
            }

            var saveBool = Configs.CassioConfig.Item("").GetValue<bool>(); //todo menu
            var saveMinHealth = Configs.CassioConfig.Item("").GetValue<Slider>().Value;
            var rTar =
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(h => Spells[SpellSlot.R].WillHit(h, Player.ServerPosition) && !h.IsInvulnerable)
                    .OrderBy(h => h.Distance(Player)).First();
            var rTarcastPred = Spells[SpellSlot.R].GetPrediction(rTar, true, Spells[SpellSlot.R].Range);

            if (saveBool && Player.HealthPercentage() < saveMinHealth)
            {
                Instances.CastR(rTar.IsFacing(Player), rTarcastPred.CastPosition);
            }

        }
        private static void Harass()
        {
            var t = TargetSelector.GetTarget(Spells[SpellSlot.Q].Range, TargetSelector.DamageType.Magical);
            var qDelay = Configs.CassioConfig.Item("apollo.cassio.delays.harass").GetValue<bool>()
                ? Instances.Delay(SpellSlot.Q)
                : 0;
            var wDelay = Configs.CassioConfig.Item("apollo.cassio.delays.harass").GetValue<bool>()
                ? Instances.Delay(SpellSlot.W)
                : 0;
            var eDelay = Configs.CassioConfig.Item("apollo.cassio.delays.harass").GetValue<bool>()
                ? Instances.Delay(SpellSlot.E)
                : 0;
            var mana = ObjectManager.Player.ManaPercentage() > Configs.CassioConfig.Item("apollo.cassio.manamanager.harass.mana").GetValue<Slider>().Value;
            if (!mana)
            {
                return;
            }

            if (t != null)
            {
                if (Instances.UseSpell(SpellSlot.Q) && Environment.TickCount > qDelay + Last.Q)
                    Spells[SpellSlot.Q].CastIfHitchanceEquals(t, Instances.HitChance(SpellSlot.Q), PacketCast);
                if (Instances.UseSpell(SpellSlot.W) && Environment.TickCount > wDelay + Last.W)
                    Spells[SpellSlot.W].CastIfHitchanceEquals(t, Instances.HitChance(SpellSlot.W), PacketCast);
                if (Instances.UseSpell(SpellSlot.E) && t.HasBuffOfType(BuffType.Poison) &&
                    Environment.TickCount > eDelay + Last.E)
                    Spells[SpellSlot.E].Cast(t, PacketCast);
            }
        }
        private static void LaneClear() //todo check if all works
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells[SpellSlot.Q].Range, MinionTypes.All, MinionTeam.NotAlly);
            var minionsJung = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells[SpellSlot.Q].Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).ToList();
            var farmLocationQ = MinionManager.GetBestCircularFarmLocation(minions.Select(m => m.ServerPosition.To2D()).ToList(), Spells[SpellSlot.Q].Width, Spells[SpellSlot.Q].Range);
            var farmLocationW = MinionManager.GetBestCircularFarmLocation(minions.Select(m => m.ServerPosition.To2D()).ToList(), Spells[SpellSlot.W].Width * 0.8f, Spells[SpellSlot.W].Range);
            var mana = ObjectManager.Player.ManaPercentage() > Configs.CassioConfig.Item("apollo.cassio.manamanager.laneclear.mana").GetValue<Slider>().Value;
            if (!mana)
            {
                return;
            }

            if (farmLocationQ.MinionsHit >= 2 && Instances.UseSpell(SpellSlot.Q))
            {
                Spells[SpellSlot.Q].Cast(farmLocationQ.Position, PacketCast);
            }
            if (farmLocationW.MinionsHit >= 2 && Instances.UseSpell(SpellSlot.W))
            {
                Spells[SpellSlot.W].Cast(farmLocationW.Position, PacketCast);
            }

            var minionE = minions.Where(h => h.IsValidTarget(Spells[SpellSlot.E].Range)).OrderBy(h => h.Health).ToList().First();
            if (minionE != null)
            {
                if (minionE.Health <= Spells[SpellSlot.E].GetDamage(minionE) - 10 &&
                   Instances.UseSpell(SpellSlot.E) &&
                   HealthPrediction.GetHealthPrediction(
                       minionE, (int)(Player.Distance(minionE) / Spells[SpellSlot.E].Speed),
                       (int)(Spells[SpellSlot.E].Delay * (1000 + Game.Ping) / 2)) > 0)
                {
                    if (minionE.HasBuffOfType(BuffType.Poison))
                        Spells[SpellSlot.E].CastOnUnit(minionE, PacketCast);
                    else if (Configs.CassioConfig.Item("apollo.cassio.e.minkill").GetValue<bool>())
                        Spells[SpellSlot.E].CastOnUnit(minionE, PacketCast);

                    if (minionE.BaseSkinName.Contains("Siege") &&
                        Configs.CassioConfig.Item("apollo.cassio.e.mincanonkill").GetValue<bool>())
                        Spells[SpellSlot.E].CastOnUnit(minionE, PacketCast);
                }
            }
            if (minionsJung.First() != null)
            {
                var minion = minionsJung.First();

                if (minion.IsValidTarget() && Instances.UseSpell(SpellSlot.E))
                {
                    if (minion.HasBuffOfType(BuffType.Poison))
                        Spells[SpellSlot.E].CastOnUnit(minion, PacketCast);
                    else if (minion.Health <= Spells[SpellSlot.E].GetDamage(minion))
                        Spells[SpellSlot.E].CastOnUnit(minion, PacketCast);
                }
                if (minion.IsValidTarget(Spells[SpellSlot.Q].Range) && Instances.UseSpell(SpellSlot.Q))
                {
                    Spells[SpellSlot.Q].Cast(minion, PacketCast);
                }
                if (minion.IsValidTarget(Spells[SpellSlot.W].Range) && Instances.UseSpell(SpellSlot.W))
                {
                    Spells[SpellSlot.W].Cast(minion, PacketCast);
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) {return;}

            switch (args.SData.Name)
            {
                case "CassiopeiaNoxiousBlast":
                {
                    Last.Q = Environment.TickCount;
                    break;
                }
                case "CassiopeiaMiasma":
                {
                    Last.W = Environment.TickCount;
                    break;
                }
                case "CassiopeiaTwinFang":
                {
                    Last.E = Environment.TickCount;
                    break;
                }
            }
        }
    }
}
