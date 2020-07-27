using System;
using Launcher.Localization.Loc;
using Launcher.XamlThemes.Theming;

namespace Launcher.Core.Shared
{
    [Serializable]
    public class LauncherSettings
    {
        [NonSerialized]
        private bool _isDefault = false;

        public ThemeEnum Theme { get; set; }
        public AccentEnum Accent { get; set; }

        public LocalizationEnum Localization { get; set; }
        public bool UnfoldGameWindow { get; set; }
        public string PathToZClient { get; set; }
        public bool RunZClient { get; set; }
        public bool UseDiscordPresence { get; set; }
        public double CardTransparency { get; set; }
        public bool TryToConnect { get; set; }
        public bool DisableChangelogAutoOpen { get; set; }

        public bool IsDefault { get => _isDefault; private set => _isDefault = value; }

        static LauncherSettings()
        {
            Default = new LauncherSettings
            {
                Theme = ThemeEnum.Dark,
                Accent = AccentEnum.OrangeRed,
                Localization = LocalizationEnum.EN,
                PathToZClient = string.Empty,
                RunZClient = false,
                UnfoldGameWindow = false,
                UseDiscordPresence = false,
                CardTransparency = .1d,
                TryToConnect = false,
                IsDefault = true,
                DisableChangelogAutoOpen = false,
            };
        }

        public static LauncherSettings Default { get; }
    }
}