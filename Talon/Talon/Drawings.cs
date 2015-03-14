using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;

namespace Talon
{
    internal class Drawings
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static readonly Dictionary<SpellSlot, Spell> Spell = Spells.Spell;

        public static void Init()
        {
            DamageInd();
            Drawing.OnDraw += OnDraw;
        }

        private static void DamageInd()
        {
            var drawComboDamageBool = Config.TalonConfig.Item("apollo.talon.draw.ind.bool");
            var drawComboDamageFill = Config.TalonConfig.Item("apollo.talon.draw.ind.fill");
            DamageIndicator.DamageToUnit = Damages.ComboDmg;
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

            var drawCd = Config.TalonConfig.Item("apollo.talon.draw.cd").GetValue<Circle>();

            var cdW = !Spell[SpellSlot.W].IsReady() && drawCd.Active;
            var drawW = Config.TalonConfig.Item("apollo.talon.draw.w").GetValue<Circle>();
            if (drawW.Active && Spell[SpellSlot.W].Level > 0)
                Render.Circle.DrawCircle(Player.Position, Spell[SpellSlot.W].Range, cdW ? drawCd.Color : drawW.Color);

            var cdE = !Spell[SpellSlot.E].IsReady() && drawCd.Active;
            var drawE = Config.TalonConfig.Item("apollo.talon.draw.e").GetValue<Circle>();
            if (drawE.Active && Spell[SpellSlot.E].Level > 0)
                Render.Circle.DrawCircle(Player.Position, Spell[SpellSlot.E].Range, cdE ? drawCd.Color : drawE.Color);

            var cdR = !Spell[SpellSlot.R].IsReady() && drawCd.Active;
            var drawR = Config.TalonConfig.Item("apollo.talon.draw.r").GetValue<Circle>();
            if (drawR.Active && Spell[SpellSlot.R].Level > 0)
                Render.Circle.DrawCircle(Player.Position, Spell[SpellSlot.R].Range, cdR ? drawCd.Color : drawR.Color);
        }
    }
}
