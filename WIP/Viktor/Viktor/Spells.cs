using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Viktor
{
    internal class Spells
    {
        public static readonly int ECastRange = 525;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static readonly Dictionary<SpellSlot, Spell> Spell = new Dictionary<SpellSlot, Spell>
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, Player.Spellbook.GetSpell(SpellSlot.Q).SData.CastRange) },
            { SpellSlot.W, new Spell(SpellSlot.W, Player.Spellbook.GetSpell(SpellSlot.W).SData.CastRange) },
            { SpellSlot.E, new Spell(SpellSlot.E, 700f) },
            { SpellSlot.R, new Spell(SpellSlot.R, Player.Spellbook.GetSpell(SpellSlot.R).SData.CastRange) }
        };
        public static void Init()
        {
            Spell[SpellSlot.Q].SetTargetted(
                Player.Spellbook.GetSpell(SpellSlot.Q).SData.SpellCastTime,
                Player.Spellbook.GetSpell(SpellSlot.Q).SData.MissileSpeed);
            Spell[SpellSlot.W].SetSkillshot(
                Player.Spellbook.GetSpell(SpellSlot.W).SData.SpellCastTime,
                Player.Spellbook.GetSpell(SpellSlot.W).SData.LineWidth,
                Player.Spellbook.GetSpell(SpellSlot.W).SData.MissileSpeed, false, SkillshotType.SkillshotCircle);
            Spell[SpellSlot.E].SetSkillshot(
                Player.Spellbook.GetSpell(SpellSlot.E).SData.SpellCastTime,
                Player.Spellbook.GetSpell(SpellSlot.E).SData.LineWidth,
                Player.Spellbook.GetSpell(SpellSlot.E).SData.MissileSpeed, false, SkillshotType.SkillshotLine);
            Spell[SpellSlot.R].SetSkillshot(
                Player.Spellbook.GetSpell(SpellSlot.R).SData.SpellCastTime,
                Player.Spellbook.GetSpell(SpellSlot.R).SData.LineWidth,
                Player.Spellbook.GetSpell(SpellSlot.R).SData.MissileSpeed, false, SkillshotType.SkillshotCircle);
        }
    }
}
