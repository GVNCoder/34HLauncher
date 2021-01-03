using System;
using System.Threading.Tasks;

using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Shared;

using Zlo4NET.Api;
using Zlo4NET.Api.Service;

namespace Launcher.Core.Data
{
    public class TestRangeGameWorker : IGameWorker
    {
        public TestRangeGameWorker(
            GameSetting gameSettings,
            BaseJoinParams param,
            IZApi api,
            ISettingsService settingsService,
            IDiscord discord)
        {
        }

        public event EventHandler<GameWorkerErrorEventArgs> Error;
        public event EventHandler<GamaWorkerPipeLogEventArgs> GamePipe;
        public event EventHandler CanCloseGame;
        public event EventHandler GameLoadingCompleted;
        public event EventHandler Complete;

        public Task Begin(IZGameProcess gameProcess, GameSetting gameSettings)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}