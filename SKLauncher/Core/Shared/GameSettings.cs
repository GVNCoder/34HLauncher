using System;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Shared
{
    [Serializable]
    public class GameSettings
    {
        [NonSerialized]
        private bool _isDefault = false;

        public GameSetting[] Settings { get; set; }
        
        public bool IsDefault { get => _isDefault; private set => _isDefault = value; }

        static GameSettings()
        {
            var architecture = Environment.Is64BitOperatingSystem
                ? ZGameArchitecture.x64
                : ZGameArchitecture.x32;

            Default = new GameSettings
            {
                Settings = new[]
                {
                    new GameSetting
                    {
                        Game = ZGame.BF3,
                        PreferredArchitecture = architecture,
                        Dlls = { }
                    },
                    new GameSetting
                    {
                        Game = ZGame.BF4,
                        PreferredArchitecture = architecture,
                        Dlls = { }
                    },
                    new GameSetting
                    {
                        Game = ZGame.BFH,
                        PreferredArchitecture = architecture,
                        Dlls = { }
                    }
                },
                IsDefault = true
            };
        }

        public static GameSettings Default { get; }
    }
}