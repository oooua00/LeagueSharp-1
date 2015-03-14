using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Talon
{
    internal class Damages
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;
        public static class Dmg
        {
            static readonly int[] QDmg = { 40, 80, 120, 160, 200 };
            static readonly int[] WDmg = { 60, 110, 160, 210, 260 };
            static readonly int[] RDmg = { 240, 340, 440 };
            public static float Q(Obj_AI_Base enemy)
            {
                return
                    (float)
                        Player.CalcDamage(
                            enemy, Damage.DamageType.Physical,
                            QDmg[Spell[SpellSlot.Q].Level] + (Player.TotalAttackDamage * 1.3));
            }

            public static float W(Obj_AI_Base enemy)
            {
                return
                    (float)
                        Player.CalcDamage(
                            enemy, Damage.DamageType.Physical,
                            WDmg[Spell[SpellSlot.W].Level] + (Player.TotalAttackDamage * 1.2));
            }

            public static float R(Obj_AI_Base enemy)
            {
                return
                    (float)
                        Player.CalcDamage(
                            enemy, Damage.DamageType.Physical,
                            RDmg[Spell[SpellSlot.R].Level] + (Player.TotalAttackDamage * 1.5));
            }
        }
        public static class ManaCost
        {
            static readonly int[] QMana = { 40, 45, 50, 55, 60 };
            static readonly int[] WMana = { 60, 65, 70, 75, 80 };
            static readonly int[] EMana = { 35, 40, 45, 50, 55 };
            static readonly int[] RMana = { 80, 90, 100 };

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

        public static float ComboDmg(Obj_AI_Base enemy)
        {
            var dmg = 0d;
            var mana = Player.Mana;
            var usedMana = 0f;

            if (Spell[SpellSlot.W].IsReady() && mana >= usedMana + ManaCost.W)
            {
                dmg += Dmg.W(enemy);
                usedMana += ManaCost.W;
            }
            if (Spell[SpellSlot.Q].IsReady() && mana >= usedMana + ManaCost.Q)
            {
                dmg += Dmg.Q(enemy);
                usedMana += ManaCost.Q;
            }
            if (Spell[SpellSlot.R].IsReady() && mana >= usedMana + ManaCost.R)
            {
                dmg += Dmg.R(enemy);
            }

            dmg += Player.CalcDamage(enemy, Damage.DamageType.Physical, Player.TotalAttackDamage);

            if (Mechanics.IgniteSlot != SpellSlot.Unknown && Mechanics.IgniteSlot.IsReady())
            {
                dmg += Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return (float)dmg;
        }
    }
}
