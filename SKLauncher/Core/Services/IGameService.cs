using System;
using System.Threading.Tasks;
using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public interface IGameService
    {
        Task Run(RunContext context);
        void TryDetect();

        event EventHandler<GameEventArgs> GameEvent;
    }
}