using System;

namespace Launcher.Core.Data
{
    public class GameWorkerStateChangedEventArgs : EventArgs
    {
        public bool IsGameLoading { get; }
        public string GamePlayModeName { get; } // Multiplayer or Singleplayer or ...
        public string PlaceName { get; } // Server name or Campaign or Test Range

        public GameWorkerStateChangedEventArgs(bool isGameLoading, string gamePlayModeName, string placeName)
        {
            IsGameLoading = isGameLoading;
            GamePlayModeName = gamePlayModeName;
            PlaceName = placeName;
        }
    }
}