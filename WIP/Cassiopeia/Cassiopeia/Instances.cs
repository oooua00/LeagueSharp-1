using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Cassiopeia
{
    internal class Instances
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        public static int Delay(SpellSlot spell)
        {
            switch (Configs.CassioConfig.Item("apollo.cassio.delays.mode").GetValue<StringList>().SelectedIndex)
            {
                case  0:
                {
                    var delay = 0;
                    switch (spell)
                    {
                        case SpellSlot.Q:
                        {
                            delay = Configs.CassioConfig.Item("apollo.cassio.delays.single.q").GetValue<Slider>().Value;
                            break;
                        }
                        case SpellSlot.W:
                        {
                            delay = Configs.CassioConfig.Item("apollo.cassio.delays.single.w").GetValue<Slider>().Value;
                            break;
                        }
                        case SpellSlot.E:
                        {
                            delay = Configs.CassioConfig.Item("apollo.cassio.delays.single.e").GetValue<Slider>().Value;
                            break;
                        }
                    }
                    return delay;
                }
                case 1:
                {
                    return Configs.CassioConfig.Item("apollo.cassio.delays.all.delay").GetValue<Slider>().Value;
                }
            }
            return 0;
        }

        public static HitChance HitChance(SpellSlot spell)
        {
            var hit = LeagueSharp.Common.HitChance.High;
            switch (spell)
            {
                case SpellSlot.Q:
                    {
                        hit = (HitChance)(Configs.CassioConfig.Item("apollo.cassio.q.hitchance").GetValue<StringList>().SelectedIndex + 3);
                        break;
                    }
                case SpellSlot.W:
                {
                    hit = (HitChance)(Configs.CassioConfig.Item("apollo.cassio.w.hitchance").GetValue<StringList>().SelectedIndex + 3);
                    break;
                }
                    
            }
            return hit;
        }

        public static bool UseSpell(SpellSlot spell)
        {
            if (Configs.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                switch (spell)
                {
                    case SpellSlot.Q:
                        return Configs.CassioConfig.Item("apollo.cassio.q.modes.combo").GetValue<bool>();
                    case SpellSlot.W:
                        return Configs.CassioConfig.Item("apollo.cassio.w.modes.combo").GetValue<bool>();
                    case SpellSlot.E:
                        return Configs.CassioConfig.Item("apollo.cassio.e.modes.combo").GetValue<bool>();
                    case SpellSlot.R:
                        return Configs.CassioConfig.Item("apollo.cassio.r.modes.combo").GetValue<bool>();
                }
            }
            else if (Configs.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                switch (spell)
                {
                    case SpellSlot.Q:
                        return Configs.CassioConfig.Item("apollo.cassio.q.modes.laneclear").GetValue<bool>();
                    case SpellSlot.W:
                        return Configs.CassioConfig.Item("apollo.cassio.w.modes.laneclear").GetValue<bool>();
                    case SpellSlot.E:
                        return Configs.CassioConfig.Item("apollo.cassio.e.modes.laneclear").GetValue<bool>();
                }
            }
            else if (Configs.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                switch (spell)
                {
                    case SpellSlot.Q:
                        return Configs.CassioConfig.Item("apollo.cassio.q.modes.harass").GetValue<bool>();
                    case SpellSlot.W:
                        return Configs.CassioConfig.Item("apollo.cassio.w.modes.harass").GetValue<bool>();
                    case SpellSlot.E:
                        return Configs.CassioConfig.Item("apollo.cassio.e.modes.harass").GetValue<bool>();
                }
            }

            return false;
        }

        public static void CastR(bool faceing, Vector3 t)
        {
            if (faceing)
                SpellClass.Spells[SpellSlot.Q].Cast(
                    t, Configs.CassioConfig.Item("apollo.cassio.packetcast").GetValue<bool>());
            else
            {
                var pos = Player.ServerPosition.Extend(t, -1);

                Player.IssueOrder(GameObjectOrder.MoveTo, pos);

                SpellClass.Spells[SpellSlot.Q].Cast(
                    t, Configs.CassioConfig.Item("apollo.cassio.packetcast").GetValue<bool>());
            }
        }
        public static float GetPoisonBuffEndTime(Obj_AI_Base target)
        {
            var buffEndTime = target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Type == BuffType.Poison)
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault();
            return buffEndTime;
        }
    }
}
