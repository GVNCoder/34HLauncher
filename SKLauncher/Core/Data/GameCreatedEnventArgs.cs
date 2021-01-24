using System;

namespace Launcher.Core.Data
{
    public class GameCreatedEnventArgs : EventArgs
    {
        public IGameWorker Worker { get; }
        public string GamePlayModeName { get; }
        public string PlacementName { get; }

        public GameCreatedEnventArgs(IGameWorker worker, string gamePlayModeName, string placementName)
        {
            Worker = worker;
            GamePlayModeName = gamePlayModeName;
            PlacementName = placementName;
        }
    }
}