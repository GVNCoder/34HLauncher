using System;

namespace Launcher.Core.Data
{
    public interface IGame
    {
        bool IsRun { get; }

        bool TryClose();
        bool TryUnfoldWindow();

        event EventHandler<string> PipeLog;
    }
}