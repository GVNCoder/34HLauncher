using System;

namespace Launcher.Core.Data.Updates
{
    public class UpdateErrorEventArgs : EventArgs
    {
        public string Message { get; }

        public UpdateErrorEventArgs(string message)
        {
            Message = message;
        }
    }
}