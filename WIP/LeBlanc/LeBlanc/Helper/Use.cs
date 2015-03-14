using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc.Helper
{
    internal class Use
    {
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;
        public static bool Q(Orbwalking.OrbwalkingMode mode)
        {
            switch (mode)
            {
                    case Orbwalking.OrbwalkingMode.Combo:
                    return Config.LeBlanc.Item("apollo.leblanc.combo.q.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;

                    case Orbwalking.OrbwalkingMode.LaneClear:
                    return Config.LeBlanc.Item("apollo.leblanc.laneclear.q.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear;

                    case Orbwalking.OrbwalkingMode.Mixed:
                    return Config.LeBlanc.Item("apollo.leblanc.harass.q.bool").GetValue<bool>() &&
                           (Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed ||
                            Config.LeBlanc.Item("apollo.leblanc.harass.key").GetValue<KeyBind>().Active);
            }
            return false;
        }
        public static bool W(Orbwalking.OrbwalkingMode mode)
        {
            switch (mode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    return Config.LeBlanc.Item("apollo.leblanc.combo.w.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    return Config.LeBlanc.Item("apollo.leblanc.laneclear.w.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear;

                case Orbwalking.OrbwalkingMode.Mixed:
                    return Config.LeBlanc.Item("apollo.leblanc.harass.w.bool").GetValue<bool>() &&
                           (Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed ||
                            Config.LeBlanc.Item("apollo.leblanc.harass.key").GetValue<KeyBind>().Active);

                case Orbwalking.OrbwalkingMode.None:
                    return Config.LeBlanc.Item("apollo.leblanc.flee.key").GetValue<KeyBind>().Active &&
                           Config.LeBlanc.Item("apollo.leblanc.flee.w.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None;
            }
            return false;
        }
        public static bool W2(Orbwalking.OrbwalkingMode mode)
        {
            switch (mode)
            {
                case Orbwalking.OrbwalkingMode.LaneClear:
                    return Config.LeBlanc.Item("apollo.leblanc.laneclear.w2.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear;

                case Orbwalking.OrbwalkingMode.Mixed:
                    return Config.LeBlanc.Item("apollo.leblanc.harass.w2.bool").GetValue<bool>() &&
                           (Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed ||
                            Config.LeBlanc.Item("apollo.leblanc.harass.key").GetValue<KeyBind>().Active);
            }
            return false;
        }
        public static bool E(Orbwalking.OrbwalkingMode mode)
        {
            switch (mode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    return Config.LeBlanc.Item("apollo.leblanc.combo.e.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;

                case Orbwalking.OrbwalkingMode.Mixed:
                    return Config.LeBlanc.Item("apollo.leblanc.harass.e.bool").GetValue<bool>() &&
                           (Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed ||
                            Config.LeBlanc.Item("apollo.leblanc.harass.key").GetValue<KeyBind>().Active);

                case Orbwalking.OrbwalkingMode.None:
                    return Config.LeBlanc.Item("apollo.leblanc.flee.key").GetValue<KeyBind>().Active &&
                           Config.LeBlanc.Item("apollo.leblanc.flee.e.bool").GetValue<bool>() &&
                           Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None;
            }
            return false;
        }
        public class R
        {
            public static bool Bool(Orbwalking.OrbwalkingMode mode)
            {
                switch (mode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        return Config.LeBlanc.Item("apollo.leblanc.combo.r.bool").GetValue<bool>() &&
                               Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;
                }
                return false;
            }
            public static bool W(Orbwalking.OrbwalkingMode mode)
            {
                switch (mode)
                {
                    case Orbwalking.OrbwalkingMode.None:
                        return Config.LeBlanc.Item("apollo.leblanc.flee.key").GetValue<KeyBind>().Active &&
                               Config.LeBlanc.Item("apollo.leblanc.flee.r.w.bool").GetValue<bool>() &&
                               Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None;
                }
                return false;
            }
        }
    }
}
