using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Cassiopeia
{
    internal class Damages
    {
        private static readonly Dictionary<SpellSlot, Spell> Spells = SpellClass.Spells;
        static float QDmg(Obj_AI_Base enemy)
        {
            return Spells[SpellSlot.Q].GetDamage(enemy);
        }
        static float WDmg(Obj_AI_Base enemy)
        {
            return Spells[SpellSlot.W].IsReady() ? Spells[SpellSlot.W].GetDamage(enemy) : 0;
        }
        static float EDmg(Obj_AI_Base enemy)
        {
            return Spells[SpellSlot.E].GetDamage(enemy);
        }
        static float RDmg(Obj_AI_Base enemy)
        {
            return Spells[SpellSlot.R].IsReady() ? Spells[SpellSlot.R].GetDamage(enemy) : 0;
        }

        public static float ComboDmg(Obj_AI_Base enemy)
        {
            var dmg = 0d;

            dmg += QDmg(enemy);
            dmg += WDmg(enemy);
            dmg += EDmg(enemy) * 3;

            if (Spells[SpellSlot.R].IsReady())
            {
                dmg += QDmg(enemy);
                dmg += EDmg(enemy);
                dmg += RDmg(enemy);
            }

            return (float)dmg;
        }
    }
}
