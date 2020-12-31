using System;

namespace Launcher.Core.Data
{
    public interface IGameWorker
    {
        void Begin();
        void Stop();

        event EventHandler<GameWorkerStateChangedEventArgs> StateChanged;
        event EventHandler Complete;
    }
}