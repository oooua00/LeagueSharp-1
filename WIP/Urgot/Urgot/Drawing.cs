using System;
using System.Collections.Generic;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;

namespace Urgot
{
    internal class Drawing
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;

        public static void Init()
        {
            DamageInd();
            LeagueSharp.Drawing.OnDraw += OnDraw;
        }

        private static void DamageInd()
        {
            DamageIndicator.DamageToUnit = Damages.ComboDmg;
        }

        private static void OnDraw(EventArgs args)
        {
            if (args == null || Player.IsDead)
                return;

            var drawCd = Config.Urgot.Item("apollo.urgot.draw.cd").GetValue<Circle>();

            var cdQ = !Spell[SpellSlot.Q].IsReady() && drawCd.Active;
            var drawQ = Config.Urgot.Item("apollo.urgot.draw.q").GetValue<Circle>();
            if (drawQ.Active && Spell[SpellSlot.Q].Level > 0)
                Render.Circle.DrawCircle(Player.Position, Spell[SpellSlot.Q].Range, cdQ ? drawCd.Color : drawQ.Color);

            var cdW = !Spell[SpellSlot.W].IsReady() && drawCd.Active;
            var drawW = Config.Urgot.Item("apollo.urgot.draw.w").GetValue<Circle>();
            if (drawW.Active && Spell[SpellSlot.W].Level > 0)
                Render.Circle.DrawCircle(Player.Position, Spell[SpellSlot.W].Range, cdW ? drawCd.Color : drawW.Color);

            var cdE = !Spell[SpellSlot.E].IsReady() && drawCd.Active;
            var drawE = Config.Urgot.Item("apollo.urgot.draw.e").GetValue<Circle>();
            if (drawE.Active && Spell[SpellSlot.E].Level > 0)
                Render.Circle.DrawCircle(Player.Position, Spell[SpellSlot.E].Range, cdE ? drawCd.Color : drawE.Color);

            var cdR = !Spell[SpellSlot.R].IsReady() && drawCd.Active;
            var drawR = Config.Urgot.Item("apollo.urgot.draw.r").GetValue<Circle>();
            if (drawR.Active && Spell[SpellSlot.R].Level > 0)
                Render.Circle.DrawCircle(Player.Position, Spell[SpellSlot.R].Range, cdR ? drawCd.Color : drawR.Color);
        }
    }
}
