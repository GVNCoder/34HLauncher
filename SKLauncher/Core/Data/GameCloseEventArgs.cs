using System;

namespace Launcher.Core.Data
{
    public class GameCloseEventArgs : EventArgs
    {
        public string PipeLog { get; }

        public GameCloseEventArgs(string pipeLog)
        {
            PipeLog = pipeLog;
        }
    }
}