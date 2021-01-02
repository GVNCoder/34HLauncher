using System;

namespace Launcher.Core.Data
{
    public class GameWorkerErrorEventArgs : EventArgs
    {
        public string Message { get; }

        public GameWorkerErrorEventArgs(string message)
        {
            Message = message;
        }
    }
}