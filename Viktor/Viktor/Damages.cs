using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Viktor
{
    internal class Damages
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;

        public static class Dmg
        {
            private static readonly int[] QDmg = { 40, 60, 80, 100, 120 };
            private static readonly int[] EDmg = { 70, 115, 160, 205, 250 };
            private static readonly int[] RDmg = { 360, 670, 980 };

            private static readonly int[] QaaDmg =
            {
                20, 25, 30, 35, 40, 45, 50, 55, 60, 70, 80, 90, 110, 130, 150, 170,
                190, 210
            };

            public static float Q(Obj_AI_Base enemy)
            {
                return
                    (float)
                        Player.CalcDamage(
                            enemy, Damage.DamageType.Magical,
                            QDmg[Spell[SpellSlot.Q].Level] + (Player.TotalMagicalDamage * .2));
            }

            public static float Qaa(Obj_AI_Base enemy)
            {
                return
                    (float)
                        Player.CalcDamage(
                            enemy, Damage.DamageType.Magical,
                            QaaDmg[Player.Level] + (Player.TotalMagicalDamage * .5) + Player.TotalAttackDamage());
            }

            public static float E(Obj_AI_Base enemy)
            {
                var name = Player.Spellbook.GetSpell(SpellSlot.E).Name == "kek"; //todo get spell name
                var addDmg = name ? 1.4 : 1;
                return
                    ((float)
                        Player.CalcDamage(
                            enemy, Damage.DamageType.Magical,
                            EDmg[Spell[SpellSlot.E].Level] + (Player.TotalMagicalDamage * .7))) * (float) addDmg;
            }

            public static float R(Obj_AI_Base enemy)
            {
                return
                    (float)
                        Player.CalcDamage(
                            enemy, Damage.DamageType.Magical,
                            RDmg[Spell[SpellSlot.R].Level] + (Player.TotalMagicalDamage * 1.95));
            }
        }

        public static class ManaCost
        {
            private static readonly int[] QMana = { 45, 45, 50, 55, 60, 65 };
            private static readonly int[] WMana = { 65, 65, 65, 65, 65, 65 };
            private static readonly int[] EMana = { 70, 70, 80, 90, 100, 110 };
            private static readonly int[] RMana = { 100, 100, 100, 100 };

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
            var damage = 0d;

            if (Spell[SpellSlot.Q].IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (Spell[SpellSlot.E].IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (Spell[SpellSlot.R].IsReady())
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);
                damage += 5 * Player.GetSpellDamage(enemy, SpellSlot.R, 1);
            }

            if (Mechanics.ChaosStorm != null)
            {
                damage += Player.GetSpellDamage(enemy, SpellSlot.R, 1);
            }

            if (Mechanics.IgniteSlot != SpellSlot.Unknown &&
                Player.Spellbook.CanUseSpell(Mechanics.IgniteSlot) == SpellState.Ready)
            {
                damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return (float)damage;
        }
    }
}