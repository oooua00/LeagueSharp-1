using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace GagongSyndra
{
    internal class Menus
    {
        //Orbwalker instance
        public static Menu Menu;
        public static Menu TargetSelectorMenu;
        public static Orbwalking.Orbwalker Orbwalker;

        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static void Init()
        {
            //Base menu
            Menu = new Menu("GagongSyndra", "GagongSyndra", true);

            Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalking"));

            //Targetselector
            TargetSelectorMenu = new Menu("Target Selector", "Common_TargetSelector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Menu.AddSubMenu(TargetSelectorMenu);

            //Combo
            Menu.AddSubMenu(new Menu("Combo", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseQE", "Use QE").SetValue(true));

            //Harass
            Menu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseQH", "Use Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("HarassAAQ", "Harass with Q if enemy AA").SetValue(false));
            Menu.SubMenu("Harass")
                .AddItem(new MenuItem("HarassTurret", "Disable Harass if Inside Enemy Turret").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseWH", "Use W").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseEH", "Use E").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseQEH", "Use QE").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("HarassMana", "Only Harass if mana >").SetValue(new Slider(0)));
            Menu.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActiveT", "Harass (toggle)!").SetValue(
                        new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle, true)));

            //Farming menu:
            Menu.AddSubMenu(new Menu("Farm", "Farm"));
            Menu.SubMenu("Farm")
                .AddItem(
                    new MenuItem("UseQFarm", "Use Q").SetValue(
                        new StringList(new[] { "Freeze", "LaneClear", "Both", "No" }, 2)));
            Menu.SubMenu("Farm")
                .AddItem(
                    new MenuItem("UseWFarm", "Use W").SetValue(
                        new StringList(new[] { "Freeze", "LaneClear", "Both", "No" }, 1)));
            Menu.SubMenu("Farm")
                .AddItem(
                    new MenuItem("UseEFarm", "Use E").SetValue(
                        new StringList(new[] { "Freeze", "LaneClear", "Both", "No" }, 3)));

            //JungleFarm menu:
            Menu.AddSubMenu(new Menu("JungleFarm", "JungleFarm"));
            Menu.SubMenu("JungleFarm").AddItem(new MenuItem("UseQJFarm", "Use Q").SetValue(true));
            Menu.SubMenu("JungleFarm").AddItem(new MenuItem("UseWJFarm", "Use W").SetValue(true));
            Menu.SubMenu("JungleFarm").AddItem(new MenuItem("UseEJFarm", "Use E").SetValue(true));

            //Auto KS
            Menu.AddSubMenu(new Menu("Auto KS", "AutoKS"));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseQKS", "Use Q").SetValue(true));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseWKS", "Use W").SetValue(true));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseEKS", "Use E").SetValue(true));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseQEKS", "Use QE").SetValue(true));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseRKS", "Use R").SetValue(true));
            Menu.SubMenu("AutoKS")
                .AddItem(
                    new MenuItem("AutoKST", "AutoKS (toggle)!").SetValue(
                        new KeyBind("U".ToCharArray()[0], KeyBindType.Toggle, true)));

            //Auto Flash Kill
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseFK1", "Q+E Flash Kill").SetValue(true));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseFK2", "R Flash Kill").SetValue(true));
            Menu.SubMenu("AutoKS").AddSubMenu(new Menu("Use Flash Kill on", "FKT"));
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != Player.Team))
                Menu.SubMenu("AutoKS")
                    .SubMenu("FKT")
                    .AddItem(new MenuItem("FKT" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(true));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("MaxE", "Max Enemies").SetValue(new Slider(2, 1, 5)));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("FKMANA", "Only Flash if mana > FC").SetValue(false));

            //Misc
            Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("AntiGap", "Anti Gap Closer").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Interrupt", "Auto Interrupt Spells").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Packets", "Packet Casting").SetValue(false));
            Menu.SubMenu("Misc").AddItem(new MenuItem("IgniteALLCD", "Only ignite if all skills on CD").SetValue(false));
            if (Menu.Item("Orbwalker_Mode").GetValue<bool>()) //todo dunno
                Menu.SubMenu("Misc").AddItem(new MenuItem("OrbWAA", "AA while orbwalking").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Sound1", "Startup Sound").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Sound2", "In Game Sound").SetValue(true));
            Menu.SubMenu("Misc")
                .AddItem(new MenuItem("YasuoWall", "Don't try to use skillshots on Yasuo's Wall").SetValue(true));
            //QE Settings
            Menu.AddSubMenu(new Menu("QE Settings", "QEsettings"));
            Menu.SubMenu("QEsettings").AddItem(new MenuItem("QEDelay", "QE Delay").SetValue(new Slider(0, 0, 150)));
            Menu.SubMenu("QEsettings").AddItem(new MenuItem("QEMR", "QE Max Range %").SetValue(new Slider(100)));
            Menu.SubMenu("QEsettings")
                .AddItem(
                    new MenuItem("UseQEC", "QE to Enemy Near Cursor").SetValue(
                        new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            //R
            Menu.AddSubMenu(new Menu("R Settings", "Rsettings"));
            Menu.SubMenu("Rsettings").AddSubMenu(new Menu("Dont R if it can be killed with", "DontRw"));
            Menu.SubMenu("Rsettings")
                .SubMenu("DontRw")
                .AddItem(
                    new MenuItem("DontRwParam", "Damage From").SetValue(
                        new StringList(new[] { "All", "Either one", "None" })));
            Menu.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwQ", "Q").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwW", "W").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwE", "E").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwA", "1 x AA").SetValue(true));

            Menu.SubMenu("Rsettings").AddSubMenu(new Menu("Dont use R on", "DontR"));
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != Player.Team))
                Menu.SubMenu("Rsettings")
                    .SubMenu("DontR")
                    .AddItem(new MenuItem("DontR" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));
            Menu.SubMenu("Rsettings").AddSubMenu(new Menu("Dont use if target has", "DontRbuff"));
            Menu.SubMenu("Rsettings")
                .SubMenu("DontRbuff")
                .AddItem(new MenuItem("DontRbuffUndying", "Trynda's Ult").SetValue(true));
            Menu.SubMenu("Rsettings")
                .SubMenu("DontRbuff")
                .AddItem(new MenuItem("DontRbuffJudicator", "Kayle's Ult").SetValue(true));
            Menu.SubMenu("Rsettings")
                .SubMenu("DontRbuff")
                .AddItem(new MenuItem("DontRbuffAlistar", "Zilean's Ult").SetValue(true));
            Menu.SubMenu("Rsettings")
                .SubMenu("DontRbuff")
                .AddItem(new MenuItem("DontRbuffZilean", "Alistar's Ult").SetValue(true));
            Menu.SubMenu("Rsettings")
                .SubMenu("DontRbuff")
                .AddItem(new MenuItem("DontRbuffZac", "Zac's Passive").SetValue(true));
            Menu.SubMenu("Rsettings")
                .SubMenu("DontRbuff")
                .AddItem(new MenuItem("DontRbuffAttrox", "Attrox's Passive").SetValue(true));
            Menu.SubMenu("Rsettings")
                .SubMenu("DontRbuff")
                .AddItem(new MenuItem("DontRbuffSivir", "Sivir's Spell Shield").SetValue(true));
            Menu.SubMenu("Rsettings")
                .SubMenu("DontRbuff")
                .AddItem(new MenuItem("DontRbuffMorgana", "Morgana's Black Shield").SetValue(true));
            Menu.SubMenu("Rsettings").AddSubMenu(new Menu("OverKill target by xx%", "okR"));
            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != Player.Team))
                Menu.SubMenu("Rsettings")
                    .SubMenu("okR")
                    .AddItem(new MenuItem("okR" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(new Slider(0)));

            //Drawings
            Menu.AddSubMenu(new Menu("Drawings", "Drawing"));
            Menu.SubMenu("Drawing")
                .AddItem(new MenuItem("DrawQ", "Q Range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing")
                .AddItem(new MenuItem("DrawW", "W Range").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing")
                .AddItem(new MenuItem("DrawE", "E Range").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing")
                .AddItem(new MenuItem("DrawR", "R Range").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing")
                .AddItem(
                    new MenuItem("DrawQE", "QE Range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing")
                .AddItem(
                    new MenuItem("DrawQEC", "QE Cursor indicator").SetValue(
                        new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawQEMAP", "QE Target Parameters").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawWMAP", "W Target Parameters").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Gank", "Gankable Enemy Indicator").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawHPFill", "After Combo HP Fill").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("HUD", "Heads-up Display").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("KillText", "Kill Text").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("KillTextHP", "% HP After Combo Text").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("drawing", "Draw combo text").SetValue(false));

            //Add main menu
            Menu.AddToMainMenu();
        }
    }
}
