using LeagueSharp;
using LeagueSharp.Common;

namespace Talon
{
    internal class Loader
    {
        static void Main(string[] args)
        {
            if (args != null)
            {
                CustomEvents.Game.OnGameLoad += eventArgs =>
                {
                    if (ObjectManager.Player.ChampionName != "Talon")
                        return;

                    Config.Init();
                    Spells.Init();
                    Drawings.Init();
                    AssassinManager.Init();
                    UpdateChecker.Initialize("Apollo16/LeagueSharp/tree/master/Talon");
                    Mechanics.Init();
                    Notifications.AddNotification("Apollo's " + ObjectManager.Player.ChampionName + " Loaded", 5000);
                };
            }
        }
    }
}
