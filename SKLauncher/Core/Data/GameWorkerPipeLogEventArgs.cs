using System;

namespace Launcher.Core.Data
{
    public class GameWorkerPipeLogEventArgs : EventArgs
    {
        public string PipeLog { get; }

        public GameWorkerPipeLogEventArgs(string pipeLog)
        {
            PipeLog = pipeLog;
        }
    }
}