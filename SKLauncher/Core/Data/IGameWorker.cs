using System;
using System.Threading.Tasks;

using Launcher.Core.Shared;

using Zlo4NET.Api.Service;

namespace Launcher.Core.Data
{
    public interface IGameWorker
    {
        Task Begin(IZGameProcess gameProcess, GameSettings gameSettings, CreateGameParametersBase parameters);
        void Stop();

        event EventHandler<GameWorkerErrorEventArgs> Error;
        event EventHandler<GameWorkerPipeLogEventArgs> GamePipe;
        event EventHandler CanCloseGame;
        event EventHandler GameLoadingCompleted; 
        event EventHandler Complete;
    }
}