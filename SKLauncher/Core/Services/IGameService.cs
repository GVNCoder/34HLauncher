using System;
using System.Threading.Tasks;

using Launcher.Core.Data;

namespace Launcher.Core.Services
{
    public interface IGameService
    {
        Task RunMultiplayer(CreateMultiplayerParameters parameters);
        Task RunSingleplayer(CreateSingleplayerParameters parameters);
        Task RunPlayground(CreateTestRangeParameters parameters);
        Task RunCoop(CreateCoopParameters parameters);

        event EventHandler<GameCreatedEnventArgs> GameCreated;
        event EventHandler<GameCreationErrorEventArgs> GameCreationError;
    }
}