using LeagueSharp.Common;

namespace LeBlanc.Helper
{
    internal class GetHitchance
    {
        public static HitChance W(Orbwalking.OrbwalkingMode mode)
        {
            switch (mode)
            {
                    case Orbwalking.OrbwalkingMode.Combo:
                    return
                        (HitChance)
                            (Config.LeBlanc.Item("apollo.leblanc.combo.w.pre").GetValue<StringList>().SelectedIndex +
                             3);

                    case Orbwalking.OrbwalkingMode.Mixed:
                    return
                        (HitChance)
                            (Config.LeBlanc.Item("apollo.leblanc.harass.w.pre").GetValue<StringList>().SelectedIndex +
                             3);
            }

            return HitChance.High;
        }
        public static HitChance E(Orbwalking.OrbwalkingMode mode)
        {
            switch (mode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    return
                        (HitChance)
                            (Config.LeBlanc.Item("apollo.leblanc.combo.e.pre").GetValue<StringList>().SelectedIndex + 3);

                case Orbwalking.OrbwalkingMode.Mixed:
                    return
                        (HitChance)
                            (Config.LeBlanc.Item("apollo.leblanc.harass.e.pre").GetValue<StringList>().SelectedIndex + 3);

                case Orbwalking.OrbwalkingMode.None:
                    return
                        (HitChance)
                            (Config.LeBlanc.Item("apollo.leblanc.flee.e.pre").GetValue<StringList>().SelectedIndex + 3);
            }

            return HitChance.High;
        }
        public class R
        {
            public static HitChance W(Orbwalking.OrbwalkingMode mode)
            {
                switch (mode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        return
                            (HitChance)
                                (Config.LeBlanc.Item("apollo.leblanc.combo.r.w.pre").GetValue<StringList>().SelectedIndex +
                                 3);
                }

                return HitChance.High;
            }
            public static HitChance E(Orbwalking.OrbwalkingMode mode)
            {
                switch (mode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        return
                            (HitChance)
                                (Config.LeBlanc.Item("apollo.leblanc.combo.r.e.pre").GetValue<StringList>().SelectedIndex +
                                 3);
                }

                return HitChance.High;
            }
        }
    }
}
