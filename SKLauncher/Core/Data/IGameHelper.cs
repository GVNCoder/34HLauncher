using System;

namespace Launcher.Core.Data
{
    public interface IGameHelper
    {
        void BeginWork();
        event EventHandler<GameCloseEventArgs> Close;
    }
}