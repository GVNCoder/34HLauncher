using System;

namespace Launcher.Core.Shared
{
    public class NetworkEventArgs : EventArgs
    {
        public bool ConnectionState { get; }

        public NetworkEventArgs(bool state)
        {
            ConnectionState = state;
        }
    }
}