using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc
{
    internal class Loader
    {
        private static void Main(string[] args)
        {
            if (args != null)
            {
                CustomEvents.Game.OnGameLoad += eventArgs =>
                {
                    if (ObjectManager.Player.ChampionName != "Leblanc")
                        return;

                    Configs.Init();
                    Spells.Init();
                    Drawings.Init();
                    Objects.Init();
                    LeBlanc.Init();

                    Game.PrintChat("<b><font color =\"#FFFFFF\">Princess LeBlanc</font></b><font color =\"#FFFFFF\"> by </font><b><font color=\"#FF66FF\">Leia</font></b><font color =\"#FFFFFF\"> loaded!</font>");
                };
            }
        }
    }
}
