using System;
using System.Threading.Tasks;

using Launcher.Core.Data;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Services
{
    public interface IGameService
    {
        Task RunMultiplayer(MultiplayerJoinParams parameters);
        Task RunSingleplayer(SingleplayerJoinParams parameters);
        Task RunPlayground(TestRangeJoinParams parameters);
        Task RunCoop(CoopJoinParams parameters);

        event EventHandler<GameCreatedEnventArgs> GameCreated;
        event EventHandler<GameCreationErrorEventArgs> GameCreationError;

        ZPlayMode CurrentPlayMode { get; }
    }
}