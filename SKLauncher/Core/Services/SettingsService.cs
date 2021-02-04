using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private const string _launcherSettingsFileName = _settingsDirectoryName + "\\launcher.settings.34h";
        private const string _gameSettingsFileName = _settingsDirectoryName + "\\game.settings.34h";
        private const string _settingsDirectoryName = "settings";

        private LauncherSettings _launcherSettings;
        private GameSettings _gameSettings;

        private T _loadSettingsByPath<T>(IFormatter formatter, string path) where T: class
        {
            T var;
            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    var = (T) formatter.Deserialize(fs);
                }
            }
            catch
            {
                var = null;
            }

            return var;
        }

        private bool _saveSettingsByPath<T>(IFormatter formatter, string path, T setting)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, setting);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public GameSettings GetGameSettings() => _gameSettings;

        public LauncherSettings GetLauncherSettings() => _launcherSettings;

        public bool LoadLauncherSettings()
        {
            //if (!Directory.Exists(_settingsDirectoryName))
            //{
            //    Directory.CreateDirectory(_settingsDirectoryName);
            //}

            var formatter = new BinaryFormatter();
            var launcherSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _launcherSettingsFileName);

            var launcherSettings = _loadSettingsByPath<LauncherSettings>(formatter, launcherSettingsPath);
            if (launcherSettings != null)
            {
                _launcherSettings = launcherSettings;
                return true;
            }
            else
            {
                _launcherSettings = LauncherSettings.Default;
                return false;
            }
        }

        public bool LoadGameSettings()
        {
            if (!Directory.Exists(_settingsDirectoryName))
            {
                Directory.CreateDirectory(_settingsDirectoryName);
            }

            var formatter = new BinaryFormatter();
            var gameSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _gameSettingsFileName);

            var gameSettings = _loadSettingsByPath<GameSettings>(formatter, gameSettingsPath);
            if (gameSettings != null)
            {
                _gameSettings = gameSettings;
                return true;
            }
            else
            {
                _gameSettings = GameSettings.Default;
                return false;
            }
        }

        public bool Save()
        {
            var formatter = new BinaryFormatter();
            var launcherSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _launcherSettingsFileName);
            var gameSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _gameSettingsFileName);

            return _saveSettingsByPath<LauncherSettings>(formatter, launcherSettingsPath, _launcherSettings)
                   && _saveSettingsByPath<GameSettings>(formatter, gameSettingsPath, _gameSettings);
        }

        public bool GlobalBlock { get; private set; }

        public void SetGlobalLock() => GlobalBlock = true;
        public void FreeGlobalLock() => GlobalBlock = false;
    }
}