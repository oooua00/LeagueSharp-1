using LeagueSharp;
using LeagueSharp.Common;

namespace LeBlanc
{
    internal class Loader
    {
        static void Main(string[] args)
        {
            if (args != null)
            {
                CustomEvents.Game.OnGameLoad += eventArgs =>
                {
                    if (ObjectManager.Player.ChampionName != "Leblanc")
                        return;

                    Config.Init();
                    Helper.Spells.Init();
                    Helper.Objects.Init();
                    Drawings.Init();
                    AssassinManager.Init();
                    UpdateChecker.Initialize("Apollo16/LeagueSharp/tree/master/LeBlanc");
                    Mechanics.Init();
                    Notifications.AddNotification("Apollo's " + ObjectManager.Player.ChampionName + " Loaded", 5000);
                };
            }
        }
    }
}
