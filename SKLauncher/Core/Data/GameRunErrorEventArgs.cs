using System;

namespace Launcher.Core.Data
{
    public class GameRunErrorEventArgs : EventArgs
    {
        public Exception Error { get; set; }

        public GameRunErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }
}