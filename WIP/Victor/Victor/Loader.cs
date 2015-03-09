using LeagueSharp;
using LeagueSharp.Common;

namespace Victor
{
    internal class Loader
    {
        private static void Main(string[] args)
        {
            if (args != null)
            {

                CustomEvents.Game.OnGameLoad += eventArgs =>
                {
                    if (ObjectManager.Player.ChampionName != "Viktor")
                        return;

                    Spells.Init();
                    Config.Init();
                    Drawing.Init();
                    Mechanics.Init();
                };
            }
        }
    }
}
