using System;

using Launcher.Localization.Loc;
using Launcher.XamlThemes.Theming;

namespace Launcher.Core.Shared
{
    // Auto...
    // Use...
    // Data...
    // DataCollection...
    // Disable...

    [Serializable]
    public class LauncherSettings
    {
        public bool AutoUnfoldGameWindow         { get; set; }
        public bool AutoRunZClient               { get; set; }
        public bool AutoConnectToZClient         { get; set; }
        public bool AutoCloseZClientWithLauncher { get; set; }

        public bool UseDiscordRPC { get; set; }

        public bool DisableChangelogOpening { get; set; }

        public string DataZClientPath              { get; set; }
        public double DataMainMenuCardTransparency { get; set; }

        public LauncherTheme        DataTheme        { get; set; }
        public LauncherLocalization DataLocalization { get; set; }

        public GameSettings[] DataCollectionGameSettings { get; set; }

        public uint[] DataCollectionHiddenServers        { get; set; }
        public uint[] DataCollectionFavoriteServers      { get; set; }

        //static LauncherSettings()
        //{
        //    Default = new LauncherSettings
        //    {
        //        Theme = ThemeEnum.Dark,
        //        Accent = AccentEnum.OrangeRed,
        //        Localization = LocalizationEnum.EN,
        //        PathToZClient = string.Empty,
        //        RunZClient = false,
        //        UnfoldGameWindow = false,
        //        UseDiscordPresence = false,
        //        CardTransparency = .1d,
        //        TryToConnect = false,
        //        IsDefault = true,
        //        DisableChangelogAutoOpen = false,
        //        DisableAskServerBrowserDiscordLeave = false,
        //        CloseZClientWithLauncher = false
        //    };
        //}

        //public static LauncherSettings Default { get; }
    }
}