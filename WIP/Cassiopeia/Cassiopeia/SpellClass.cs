using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Cassiopeia
{
    internal class SpellClass
    {
        public static readonly Dictionary<SpellSlot, Spell> Spells = new Dictionary<SpellSlot, Spell>
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 850f) },
            { SpellSlot.W, new Spell(SpellSlot.W, 850f) },
            { SpellSlot.E, new Spell(SpellSlot.E, 700f) },
            { SpellSlot.R, new Spell(SpellSlot.R, 800) }
        };

        public static void Init()
        {
            Spells[SpellSlot.Q].SetSkillshot(0.6f, 40f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Spells[SpellSlot.W].SetSkillshot(0.5f, 90f, 2500, false, SkillshotType.SkillshotCircle);

            Spells[SpellSlot.E].SetTargetted(0.2f, float.MaxValue);

            Spells[SpellSlot.R].SetSkillshot(0.6f, (float)(80 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
        }
    }
}
