using LeagueSharp.Common;

namespace Talon.Helper
{
    internal class Use
    {
        public static bool Q(Orbwalking.OrbwalkingMode mode)
        {
            switch (mode)
            {
                    case Orbwalking.OrbwalkingMode.Combo:
                    return Config.TalonConfig.Item("apollo.talon.combo.q.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;

                    case Orbwalking.OrbwalkingMode.LaneClear:
                    return Config.TalonConfig.Item("apollo.talon.laneclear.q.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear;

                    case Orbwalking.OrbwalkingMode.Mixed:
                    return Config.TalonConfig.Item("apollo.talon.harass.q.bool").GetValue<bool>() &&
                           (Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed ||
                            Config.TalonConfig.Item("apollo.talon.harass.key").GetValue<KeyBind>().Active);
            }
            return false;
        }
        public static bool W(Orbwalking.OrbwalkingMode mode)
        {
            switch (mode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    return Config.TalonConfig.Item("apollo.talon.combo.w.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    return Config.TalonConfig.Item("apollo.talon.laneclear.w.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear;

                case Orbwalking.OrbwalkingMode.Mixed:
                    return Config.TalonConfig.Item("apollo.talon.harass.w.bool").GetValue<bool>() &&
                           (Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed ||
                            Config.TalonConfig.Item("apollo.talon.harass.key").GetValue<KeyBind>().Active);
            }
            return false;
        }
        public static bool E(Orbwalking.OrbwalkingMode mode)
        {
            switch (mode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    return Config.TalonConfig.Item("apollo.talon.combo.e.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;

                case Orbwalking.OrbwalkingMode.Mixed:
                    return Config.TalonConfig.Item("apollo.talon.harass.e.bool").GetValue<bool>() &&
                           (Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed ||
                            Config.TalonConfig.Item("apollo.talon.harass.key").GetValue<KeyBind>().Active);
            }
            return false;
        }
        public static bool R(Orbwalking.OrbwalkingMode mode)
        {
            switch (mode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    return Config.TalonConfig.Item("apollo.talon.combo.r.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;
            }
            return false;
        }
    }
}
