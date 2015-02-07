using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc
{
    internal class Damages
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        private static Spell Q
        {
            get { return Spells.Q; }
        }

        private static Spell W
        {
            get { return Spells.W; }
        }

        private static Spell E
        {
            get { return Spells.E; }
        }

        private static Spell R
        {
            get { return Spells.R; }
        }

        public static float Combo(Obj_AI_Base enemy)
        {
            var qDmg = new[] { 55, 80, 105, 130, 155 }[Q.Level] + 0.4 * Player.TotalMagicalDamage();
            var name = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Name;
            var maxDmg = new[] { 200, 400, 600 }[R.Level] + 1.3 * Player.TotalMagicalDamage();
            var rqDmg = new[] { 100, 200, 300 }[R.Level] + 0.65 * Player.TotalMagicalDamage();
            var dmg = 0d;

            if (Q.IsReady())
            {
                dmg += Player.CalcDamage(enemy, Damage.DamageType.Magical, qDmg);

                if ((W.IsReady() && W.Instance.Name == "LeblancSlide") || E.IsReady() || R.IsReady())
                    dmg += Player.CalcDamage(enemy, Damage.DamageType.Magical, qDmg);
            }

            if (enemy.HasBuff("LeblancChaosOrb", true))
            {
                dmg += Player.CalcDamage(enemy, Damage.DamageType.Magical, qDmg);
            }

            if (W.IsReady())
            {
                var wDmg = new[] { 85, 125, 165, 205, 245 }[Q.Level] + 0.6 * Player.TotalMagicalDamage();

                dmg += Player.CalcDamage(enemy, Damage.DamageType.Magical, wDmg);
            }

            if (E.IsReady())
            {
                var eDmg = new[] { 40, 65, 90, 115, 140 }[Q.Level] + 0.5 * Player.TotalMagicalDamage();

                dmg += Player.CalcDamage(enemy, Damage.DamageType.Magical, 2 * eDmg);
            }

            if (R.IsReady())
            {
                switch (name)
                {
                    case "LeblancChaosOrbM":
                    {
                        dmg += Player.CalcDamage(enemy, Damage.DamageType.Magical, rqDmg);

                        if ((W.IsReady() && W.Instance.Name == "LeblancSlide") || E.IsReady() || Q.IsReady())
                        {
                            dmg += Player.CalcDamage(enemy, Damage.DamageType.Magical, maxDmg);
                        }
                        break;
                    }
                    case "LeblancSlideM":
                    {
                        var wDmg = new[] { 150, 300, 450 }[R.Level] + 0.975 * Player.TotalMagicalDamage();
                        dmg += Player.CalcDamage(enemy, Damage.DamageType.Magical, wDmg);
                        break;
                    }
                    case "LeblancSoulShackleM":
                    {
                        var eDmg = new[] { 100, 200, 300 }[R.Level] + 0.65 * Player.TotalMagicalDamage();

                        dmg += Player.CalcDamage(enemy, Damage.DamageType.Magical, eDmg);

                        if (Player.CalcDamage(enemy, Damage.DamageType.Magical, eDmg) > maxDmg)
                        {
                            dmg += Player.CalcDamage(enemy, Damage.DamageType.Magical, maxDmg);
                        }
                        break;
                    }
                }
            }
            if (Player.GetSpellSlot("SummonerIgnite") != SpellSlot.Unknown)
            {
                dmg += Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }
            if (enemy.HasBuff("LeblancChaosOrbM", true))
            {
                dmg += Player.CalcDamage(enemy, Damage.DamageType.Magical, rqDmg);
            }

            return (float) dmg;
        }
    }
}
