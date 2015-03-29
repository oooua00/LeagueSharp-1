using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc
{
    internal class Drawings
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        public static void Init()
        {
            Drawing.OnDraw += OnDraw;
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
        }
    }
}
