using LeagueSharp.Common;

namespace Talon.Helper
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
                            (Config.TalonConfig.Item("apollo.talon.combo.w.pre").GetValue<StringList>().SelectedIndex +
                             3);

                    case Orbwalking.OrbwalkingMode.Mixed:
                    return
                        (HitChance)
                            (Config.TalonConfig.Item("apollo.talon.harass.w.pre").GetValue<StringList>().SelectedIndex +
                             3);
            }

            return HitChance.High;
        }
    }
}
