using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc.Helper
{
    internal class SpellDamage
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;

        private static readonly int[] QDmg = { 55, 80, 105, 130, 155 };
        private static readonly int[] WDmg = { 85, 125, 165, 205, 245 };
        private static readonly int[] EDmg = { 40, 65, 90, 115, 140 };
        private static readonly int[] RqDmg = { 100, 200, 300 };
        private static readonly int[] RwDmg = { 150, 300, 450 };
        private static readonly int[] ReDmg = { 100, 200, 300 };

        private static readonly Damage.DamageType DmgType = Damage.DamageType.Magical;
        public static float Q(Obj_AI_Base enemy)
        {
            return
                (float)
                    Player.CalcDamage(enemy, DmgType, QDmg[Spell[SpellSlot.Q].Level] + (Player.TotalMagicalDamage * .4));
        }

        public static float W(Obj_AI_Base enemy)
        {
            return
                (float)
                    Player.CalcDamage(enemy, DmgType, WDmg[Spell[SpellSlot.W].Level] + (Player.TotalMagicalDamage * .6));
        }

        public static float E(Obj_AI_Base enemy)
        {
            return
                (float)
                    Player.CalcDamage(enemy, DmgType, EDmg[Spell[SpellSlot.E].Level] + (Player.TotalMagicalDamage * .5));
        }

        public class R
        {
            public static float Q(Obj_AI_Base enemy)
            {
                return
                    (float)
                        Player.CalcDamage(enemy, DmgType, RqDmg[Spell[SpellSlot.R].Level] + (Player.TotalMagicalDamage * .65));
            }

            public static float W(Obj_AI_Base enemy)
            {
                return
                    (float)
                        Player.CalcDamage(enemy, DmgType, RwDmg[Spell[SpellSlot.R].Level] + (Player.TotalMagicalDamage * .975));
            }

            public static float E(Obj_AI_Base enemy)
            {
                return
                    (float)
                        Player.CalcDamage(enemy, DmgType, ReDmg[Spell[SpellSlot.R].Level] + (Player.TotalMagicalDamage * .65));
            }    
        }
    }
}
