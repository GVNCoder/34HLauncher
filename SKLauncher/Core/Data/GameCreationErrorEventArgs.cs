using System;

namespace Launcher.Core.Data
{
    public class GameCreationErrorEventArgs : EventArgs
    {
        public string Message { get; }

        public GameCreationErrorEventArgs(string message)
        {
            Message = message;
        }
    }
}