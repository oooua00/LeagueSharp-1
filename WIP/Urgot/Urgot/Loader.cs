using LeagueSharp;
using LeagueSharp.Common;

namespace Urgot
{
    internal class Loader
    {
        static void Main(string[] args)
        {
            if (args != null)
            {
                CustomEvents.Game.OnGameLoad += eventArgs =>
                {
                    if (ObjectManager.Player.ChampionName != "Urgot")
                        return;

                    Spells.Init();
                    Config.Init();
                    Drawing.Init();
                    Mechanics.Init();
                    UpdateChecker.Initialize("Apollo16/LeagueSharp/tree/master/Urgot");
                    Notifications.AddNotification("Apollo's " + ObjectManager.Player.ChampionName + " Loaded", 5000);
                };
            }
        }
    }
}
