using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Urgot
{
    internal class Damages
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;
        public static class Dmg
        {
            static readonly int[] QDmg = { 10, 40, 70, 100, 130 };
            static readonly int[] EDmg = { 75, 130, 185, 240, 295 };
            public static float Q(Obj_AI_Base enemy)
            {
                return
                    (float)
                        Player.CalcDamage(
                            enemy, Damage.DamageType.Physical,
                            QDmg[Spell[SpellSlot.Q].Level] + (Player.TotalAttackDamage * .85));
            }

            public static float E(Obj_AI_Base enemy)
            {
                return
                    (float)
                        Player.CalcDamage(
                            enemy, Damage.DamageType.Physical,
                            EDmg[Spell[SpellSlot.E].Level] + (Player.TotalAttackDamage * .6));
            }
        }
        public static class ManaCost
        {
            static readonly int[] QMana = { 40, 40, 40, 40, 40, 40 };
            static readonly int WMana = (int)(Player.MaxMana * .08);
            static readonly int[] EMana = { 50, 55, 60, 65, 70 };
            static readonly int[] RMana = { 100, 100, 100 };

            public static float Q
            {
                get { return QMana[Spell[SpellSlot.Q].Level]; }
            }

            public static float W
            {
                get { return WMana; }
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

        public static float ComboDmg(Obj_AI_Base enemy)
        {
            var dmg = 0d;
            var mana = Player.Mana;
            var usedMana = 0f;

            if (Spell[SpellSlot.Q].IsReady() && mana >= usedMana + ManaCost.Q)
            {
                dmg += Dmg.Q(enemy);
                usedMana += ManaCost.Q;
            }
            if (Spell[SpellSlot.E].IsReady() && mana >= usedMana + ManaCost.E)
            {
                dmg += Dmg.E(enemy);
                usedMana += ManaCost.E;

                if (mana >= usedMana + ManaCost.Q)
                {
                    dmg += Dmg.Q(enemy);
                    usedMana += ManaCost.Q;
                }
                if (mana >= usedMana + ManaCost.Q)
                {
                    dmg += Dmg.Q(enemy);
                }
            }
            if (Mechanics.IgniteSlot != SpellSlot.Unknown && Mechanics.IgniteSlot.IsReady())
            {
                dmg += Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return (float)dmg;
        }
    }
}
