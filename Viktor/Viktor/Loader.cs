using LeagueSharp;
using LeagueSharp.Common;

namespace Viktor
{
    /// <Credits:>
    /// Hellsing:
    /// Update Checker
    /// Healthpred 
    /// 
    /// blm95:
    /// DmgInd
    /// </Credits:>
    internal class Loader
    {
        private static void Main(string[] args)
        {
            if (args != null)
            {
                Utils.ClearConsole();

                CustomEvents.Game.OnGameLoad += eventArgs =>
                {
                    if (ObjectManager.Player.ChampionName != "Viktor")
                    {
                        return;
                    }
                    Utils.ClearConsole();
                    Updater.Init("Apollo16/LeagueSharp/master/Viktor/Viktor");
                    Spells.Init();
                    Config.Init();
                    Drawing.Init();
                    Utility.HpBarDamageIndicator.DamageToUnit = Damages.ComboDmg;
                    Utility.HpBarDamageIndicator.Enabled = true;
                    Mechanics.Init();
                    Notifications.AddNotification("Apollo's " + ObjectManager.Player.ChampionName + " Loaded", 5000, false);
                };
            }
        }
    }
}