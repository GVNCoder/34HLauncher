using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Launcher.Core.Shared;
using Launcher.Helpers;
using Launcher.Localization.Loc;
using Launcher.XamlThemes.Theming;

using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private const string _SettingsFileName = "settings.34h";

        private readonly object _sync;
        private readonly LauncherSettings _defaultSettings;

        private LauncherSettings _settings;
        private bool _settingsLock;
        private bool _isDefault;

        public SettingsService()
        {
            _sync = new object();

            // create default settings
            var currentSystemArchitecture = Environment.Is64BitOperatingSystem
                ? ZGameArchitecture.x64
                : ZGameArchitecture.x32;
            var emptyStringArray = new string[] { };
            var emptyUintArray = new uint[] { };

            // create default game settings instance
            var defaultGameSettings = new[]
            {
                new GameSettings
                {
                    DataGame = ZGame.BF3,
                    DataArchitecture = currentSystemArchitecture,
                    DataCollectionInjectableDlls = emptyStringArray
                },
                new GameSettings
                {
                    DataGame = ZGame.BF4,
                    DataArchitecture = currentSystemArchitecture,
                    DataCollectionInjectableDlls = emptyStringArray
                },
                new GameSettings
                {
                    DataGame = ZGame.BFH,
                    DataArchitecture = currentSystemArchitecture,
                    DataCollectionInjectableDlls = emptyStringArray
                }
            };

            // create default settings instance
            _defaultSettings = new LauncherSettings
            {
                AutoUnfoldGameWindow = false,
                AutoRunZClient = false,
                AutoConnectToZClient = false,
                AutoCloseZClientWithLauncher = false,
                AutoOpenChangelog = true,

                UseDiscordRPC = false,

                DataZClientPath = string.Empty,
                DataMainMenuCardTransparency = .1d,

                DataTheme            = LauncherTheme.Dark,
                DataLocalization     = LauncherLocalization.EN,

                DataCollectionGameSettings = defaultGameSettings,

                DataCollectionFavoriteServers = emptyUintArray,
                DataCollectionHiddenServers = emptyUintArray
            };
        }

        public void SetLock()
        {
            lock (_sync)
            {
                _settingsLock = true;
            }
        }

        public void FreeLock()
        {
            lock (_sync)
            {
                _settingsLock = false;
            }
        }

        public bool CanGetAccess()
        {
            lock (_sync)
            {
                return _settingsLock;
            }
        }

        public bool IsDefault() => _isDefault;

        public bool Load()
        {
            // create formatter
            var formatter = new BinaryFormatter();

            // build settings path
            var settingsPath = Path.Combine(FolderConstant.SettingsFolder, _SettingsFileName);

            // try load
            try
            {
                using (var stream = new FileStream(settingsPath, FileMode.Open))
                {
                    _settings = (LauncherSettings) formatter.Deserialize(stream);
                }
            }
            catch
            {
                // setup default settings
                _settings = _defaultSettings;

                // check it
                _isDefault = true;

                return false;
            }

            return true;
        }

        public bool Save()
        {
            // create formatter
            var formatter = new BinaryFormatter();

            // build settings path
            var settingsPath = Path.Combine(FolderConstant.SettingsFolder, _SettingsFileName);

            // try save
            try
            {
                using (var stream = new FileStream(settingsPath, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(stream, _settings);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        // ReSharper disable once ConvertToAutoProperty
        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public LauncherSettings Current => _settings;
    }
}