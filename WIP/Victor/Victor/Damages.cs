using LeagueSharp;
using LeagueSharp.Common;

namespace Victor
{
    internal class Damages
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Spell Q = Spells.Spell[SpellSlot.Q];
        private static readonly Spell W = Spells.Spell[SpellSlot.W];
        private static readonly Spell E = Spells.Spell[SpellSlot.E];
        private static readonly Spell R = Spells.Spell[SpellSlot.R];
        public static class Dmg
        {
            public static float Q(Obj_AI_Base enemy)
            {
                return (float) Player.GetSpellDamage(enemy, SpellSlot.Q);
            }

            public static float W(Obj_AI_Base enemy)
            {
                return (float) Player.GetSpellDamage(enemy, SpellSlot.W);
            }

            public static float E(Obj_AI_Base enemy)
            {
                return (float) Player.GetSpellDamage(enemy, SpellSlot.E);
            }

            public static float R(Obj_AI_Base enemy)
            {
                return (float) Player.GetSpellDamage(enemy, SpellSlot.R);
            }
        }
        public static class ManaCost
        {
            public static float Q
            {
                get { return Player.GetSpell(SpellSlot.Q).ManaCost; }
            }

            public static float W
            {
                get { return Player.GetSpell(SpellSlot.W).ManaCost; }
            }

            public static float E
            {
                get { return Player.GetSpell(SpellSlot.E).ManaCost; }
            }

            public static float R
            {
                get { return Player.GetSpell(SpellSlot.R).ManaCost; }
            }
        }

        public static float ComboDmg(Obj_AI_Base enemy)
        {
            var dmg = 0d;
            var mana = Player.Mana;
            var usedMana = 0f;

            if (Q.IsReady() && mana >= usedMana + ManaCost.Q)
            {
                dmg += Dmg.Q(enemy);
                usedMana += ManaCost.Q;
            }
            if (W.IsReady() && mana >= usedMana + ManaCost.W)
            {
                dmg += Dmg.W(enemy);
                usedMana += ManaCost.W;
            }
            if (E.IsReady() && mana >= usedMana + ManaCost.E)
            {
                dmg += Dmg.E(enemy);
                usedMana += ManaCost.E;
            }
            if (R.IsReady() && mana >= usedMana + ManaCost.R)
            {
                dmg += Dmg.R(enemy);
            }

            return (float)dmg;
        }
    }
}
