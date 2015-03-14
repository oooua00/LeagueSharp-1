using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace LeBlanc
{
    internal class Drawings
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Dictionary<SpellSlot, Spell> Spell = Helper.Spells.Spell;

        public static void Init()
        {
            DamageInd();
            Drawing.OnDraw += OnDraw;
        }

        private static void DamageInd()
        {
            var drawComboDamageBool = Config.LeBlanc.Item("apollo.leblanc.draw.ind.bool");
            var drawComboDamageFill = Config.LeBlanc.Item("apollo.leblanc.draw.ind.fill");
            DamageIndicator.DamageToUnit = ComboDmg.Get;
            DamageIndicator.Enabled = drawComboDamageBool.GetValue<bool>();
            DamageIndicator.Fill = drawComboDamageFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawComboDamageFill.GetValue<Circle>().Color;
            drawComboDamageBool.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };
            drawComboDamageFill.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
        }

        private static void OnDraw(EventArgs args)
        {
            if (args == null || Player.IsDead)
                return;

            if (Helper.Objects.SecondW.Object != null)
            {
                var width = Config.LeBlanc.Item("apollo.leblanc.misc.2w.mouseover.width").GetValue<Slider>().Value;
                var wts = Drawing.WorldToScreen(Player.Position);
                var timer = (Helper.Objects.SecondW.ExpireTime - Game.Time > 0) ? (Helper.Objects.SecondW.ExpireTime - Game.Time) : 0;

                Drawing.DrawText(wts.X - 35, wts.Y + 10, Color.White, "Second W: " + timer.ToString("0.0"));
                Render.Circle.DrawCircle(Helper.Objects.SecondW.Object.Position, 100, Color.Red, width);
            }
            if (Helper.Objects.Clone.Pet != null)
            {
                var wts = Drawing.WorldToScreen(Helper.Objects.Clone.Pet.ServerPosition);
                var timer = (Helper.Objects.Clone.ExpireTime - Game.Time > 0) ? (Helper.Objects.Clone.ExpireTime - Game.Time) : 0;

                Drawing.DrawText(wts.X - 35, wts.Y + 10, Color.White, "Clone: " + timer.ToString("0.0"));
            }

            var drawCd = Config.LeBlanc.Item("apollo.leblanc.draw.cd").GetValue<Circle>();

            var cdQ = !Spell[SpellSlot.Q].IsReady() && drawCd.Active;
            var drawQ = Config.LeBlanc.Item("apollo.leblanc.draw.q").GetValue<Circle>();
            if (drawQ.Active && Spell[SpellSlot.Q].Level > 0)
                Render.Circle.DrawCircle(Player.Position, Spell[SpellSlot.Q].Range, cdQ ? drawCd.Color : drawQ.Color);

            var cdW = !Spell[SpellSlot.W].IsReady() && drawCd.Active;
            var drawW = Config.LeBlanc.Item("apollo.leblanc.draw.w").GetValue<Circle>();
            if (drawW.Active && Spell[SpellSlot.W].Level > 0)
                Render.Circle.DrawCircle(Player.Position, Spell[SpellSlot.W].Range, cdW ? drawCd.Color : drawW.Color);

            var cdE = !Spell[SpellSlot.E].IsReady() && drawCd.Active;
            var drawE = Config.LeBlanc.Item("apollo.leblanc.draw.e").GetValue<Circle>();
            if (drawE.Active && Spell[SpellSlot.E].Level > 0)
                Render.Circle.DrawCircle(Player.Position, Spell[SpellSlot.E].Range, cdE ? drawCd.Color : drawE.Color);
        }
    }
}
