using System;
using System.Threading.Tasks;
using Launcher.Core.Data;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Services
{
    public interface IGameService
    {
        Task RunMultiplayer(MultiplayerJoinParams param);
        Task RunSingleplayer(SingleplayerJoinParams param);
        Task RunPlayground(TestRangeJoinParams param);
        Task RunCoop(CoopJoinParams param);

        event EventHandler<GameCloseEventArgs> GameClose;
        event EventHandler<GameRunErrorEventArgs> GameRunError;

        bool CanRun { get; }
        BaseGameWorker CurrentGame { get; }
        ZPlayMode? CurrentPlayMode { get; }
    }
}