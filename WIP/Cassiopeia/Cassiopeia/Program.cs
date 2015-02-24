using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Cassiopeia
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args != null)
            {
                CustomEvents.Game.OnGameLoad += eventArgs =>
                {   
                    Configs.Init();
                    SpellClass.Init();
                    Mechanics.Init();
                    //todo lichbane
                    //todo Gapcloser interrupter, NonPoi minion
                };
            }
        }
    }
}
