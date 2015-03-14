using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc.Helper
{
    internal class Spells
    {
        public class Name
        {
            public const string Q = "LeblancChaosOrb";
            public const string W = "LeblancSlide";
            public const string W2 = "LeblancSlideReturn";
            public const string E = "LeblancSoulShackle";
            public class R
            {
                public const string Q = "LeblancChaosOrbM";
                public const string W = "LeblancSlideM";
                public const string W2 = "LeblancSlideReturnM";
                public const string E = "LeblancSoulShackleM";    
            }
        }


        public static readonly Dictionary<SpellSlot, Spell> Spell = new Dictionary<SpellSlot, Spell>
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q, 700f) },
            { SpellSlot.W, new Spell(SpellSlot.W, 600f) },
            { SpellSlot.E, new Spell(SpellSlot.E, 950f) },
            { SpellSlot.R, new Spell(SpellSlot.R) }
        };
        public static void Init()
        {
            Spell[SpellSlot.W].SetSkillshot(0.25f, 70, 1500, false, SkillshotType.SkillshotCircle);
            Spell[SpellSlot.E].SetSkillshot(0.25f, 70, 1600, true, SkillshotType.SkillshotLine);

            var name = Spell[SpellSlot.R].Instance.Name;
            switch (name)
            {
                case Name.R.Q:
                    {
                        Spell[SpellSlot.R] = new Spell(SpellSlot.R, Spell[SpellSlot.Q].Range);
                        break;
                    }
                case Name.R.W:
                    {
                        Spell[SpellSlot.R] = new Spell(SpellSlot.R, Spell[SpellSlot.W].Range);
                        Spell[SpellSlot.R].SetSkillshot(Spell[SpellSlot.W].Delay, Spell[SpellSlot.W].Width, Spell[SpellSlot.W].Speed, false, SkillshotType.SkillshotCircle);
                        break;
                    }
                case Name.R.E:
                    {
                        Spell[SpellSlot.R] = new Spell(SpellSlot.R, Spell[SpellSlot.E].Range);
                        Spell[SpellSlot.R].SetSkillshot(Spell[SpellSlot.E].Delay, Spell[SpellSlot.E].Width, Spell[SpellSlot.E].Speed, true, SkillshotType.SkillshotLine);
                        break;
                    }
            }
        }
    }
}
