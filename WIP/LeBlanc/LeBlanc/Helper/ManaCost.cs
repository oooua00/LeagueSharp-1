using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc.Helper
{
    internal class ManaCost
    {
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;

        private static readonly int[] QMana = { 50, 60, 70, 80, 90 };
        private static readonly int[] WMana = { 80, 85, 90, 95, 100 };
        private static readonly int[] EMana = { 80, 80, 80, 80, 80 };
        private static readonly int[] RMana = { 0, 0, 0 };

        public static float Q
        {
            get { return QMana[Spell[SpellSlot.Q].Level]; }
        }

        public static float W
        {
            get { return WMana[Spell[SpellSlot.W].Level]; }
        }

        public static float E
        {
            get { return EMana[Spell[SpellSlot.E].Level]; }
        }

        public static float R
        {
            get { return RMana[Spell[SpellSlot.R].Level]; }
        }
    }
}
