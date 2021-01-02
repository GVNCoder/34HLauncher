using System;

namespace Launcher.Core.Data
{
    public class GamaWorkerPipeLogEventArgs : EventArgs
    {
        public string PipeLog { get; }

        public GamaWorkerPipeLogEventArgs(string pipeLog)
        {
            PipeLog = pipeLog;
        }
    }
}