using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc.Helper
{
    internal class GetHealthPrediction
    {
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;
        public static bool From(Obj_AI_Base source, Obj_AI_Base t, SpellSlot spell, float dmg, bool greaterThenPre)
        {
            if (greaterThenPre)
            {
                return
                    HealthPrediction.GetHealthPrediction(
                        t, (int) (source.Distance(t) / Spell[spell].Speed),
                        (int) (Spell[spell].Delay * 1000 + Game.Ping / 2f)) < dmg;
            }
            else
            {
                return
                    HealthPrediction.GetHealthPrediction(
                        t, (int) (source.Distance(t) / Spell[spell].Speed),
                        (int) (Spell[spell].Delay * 1000 + Game.Ping / 2f)) > dmg;
            }
        }
    }
}
