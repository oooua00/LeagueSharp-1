using LeagueSharp.Common;
using System;
using System.Reflection;

namespace Viktor
{
    internal class Updater
    {
        private static readonly System.Version Version = Assembly.GetExecutingAssembly().GetName().Version;
        public static void Init(string path)
        {
            try
            {
                var data = new BetterWebClient(null).DownloadString("https://raw.github.com/" + path + "/Properties/AssemblyInfo.cs");
                foreach (var line in data.Split('\n'))
                {
                    // Skip comments
                    if (line.StartsWith("//"))
                    {
                        continue;
                    }

                    // Search for AssemblyVersion
                    if (line.StartsWith("[assembly: AssemblyVersion"))
                    {
                        var serverVersion = new System.Version(line.Substring(28, (line.Length - 4) - 28 + 1));
                        if (serverVersion > Version)
                        {
                            Notifications.AddNotification(
                                "Update avalible: " + Version + " => " + serverVersion, 5000, false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
