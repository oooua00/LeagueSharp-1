using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc_2
{
    internal class Damages
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            if (Spells.Q.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q);

            if (Spells.W.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.W);

            if (Spells.E.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.E);

            if (Spells.R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target));
        }
    }
}
