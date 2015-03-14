using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc
{
    internal class ComboDmg
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Dictionary<SpellSlot, Spell> Spell = Helper.Spells.Spell;

        public static float Get(Obj_AI_Base enemy)
        {
            var dmg = 0d;
            var mana = Player.Mana;
            var usedMana = 0f;

            if (Spell[SpellSlot.Q].IsReady() && mana >= usedMana + Helper.ManaCost.Q)
            {
                dmg += 2 * Helper.SpellDamage.Q(enemy);
                usedMana += Helper.ManaCost.Q;
            }
            if (Spell[SpellSlot.W].IsReady() && mana >= usedMana + Helper.ManaCost.W)
            {
                dmg += Helper.SpellDamage.W(enemy);
                usedMana += Helper.ManaCost.W;
            }
            if (Spell[SpellSlot.E].IsReady() && mana >= usedMana + Helper.ManaCost.E)
            {
                dmg += 2 * Helper.SpellDamage.E(enemy);
            }
            if (enemy.HasBuff(Helper.Spells.Name.R.Q, true))
            {
                dmg += Helper.SpellDamage.R.Q(enemy);
            }
            if (enemy.HasBuff(Helper.Spells.Name.Q, true))
            {
                dmg += Helper.SpellDamage.Q(enemy);
            }

            var name = Spell[SpellSlot.R].Instance.Name;
            if (Spell[SpellSlot.R].IsReady() && name != Helper.Spells.Name.R.W2)
            {
                switch (name)
                {
                    case Helper.Spells.Name.R.Q:
                    {
                        dmg += 2 * Helper.SpellDamage.R.Q(enemy);
                        break;
                    }
                    case Helper.Spells.Name.R.W:
                    {
                        dmg += Helper.SpellDamage.R.W(enemy);
                        break;
                    }
                    case Helper.Spells.Name.R.E:
                    {
                        dmg += 2 * Helper.SpellDamage.R.E(enemy);
                        break;
                    }
                }
            }

            return (float)dmg;
        }
    }
}
