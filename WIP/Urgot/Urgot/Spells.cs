using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Urgot
{
    internal class Spells
    {
        public static readonly float Q2Range = 1200;
        public static readonly Dictionary<SpellSlot, Spell> Spell = new Dictionary<SpellSlot, Spell>
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 1000f) },
            { SpellSlot.W, new Spell(SpellSlot.W) },
            { SpellSlot.E, new Spell(SpellSlot.E, 1100f) },
            { SpellSlot.R, new Spell(SpellSlot.R) }
        };
        public static void Init()
        {
            Spell[SpellSlot.Q].SetSkillshot(.125f, 60, 1600, true, SkillshotType.SkillshotLine);
            Spell[SpellSlot.E].SetSkillshot(.25f, 210, 1500, false, SkillshotType.SkillshotCircle);

            Game.OnUpdate += args =>
            {
                Spell[SpellSlot.R].Range = 400 + (150 * Spell[SpellSlot.R].Level);
            };
        }
    }
}
