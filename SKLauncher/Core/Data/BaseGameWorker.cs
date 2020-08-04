using System;
using Launcher.Core.RPC;
using Launcher.Core.Shared;

namespace Launcher.Core.Data
{
    public abstract class BaseGameWorker
    {
        protected readonly IDiscord _discord;
        protected readonly IGameControl _view;

        protected BaseGameWorker(
            IDiscord discord,
            IGameControl view)
        {
            _discord = discord;
            _view = view;
        }

        public abstract void BeginWork();
        public event EventHandler<GameCloseEventArgs> Done;
        public BaseJoinParams Params { get; protected set; }

        protected void _OnWorkDone(string pipeLog) => Done?.Invoke(this, new GameCloseEventArgs(pipeLog));
    }
}