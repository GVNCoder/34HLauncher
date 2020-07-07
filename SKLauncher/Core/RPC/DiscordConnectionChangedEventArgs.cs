using System;

namespace Launcher.Core.RPC
{
    public class DiscordConnectionChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; }

        public DiscordConnectionChangedEventArgs(bool connectionState)
        {
            IsConnected = connectionState;
        }
    }
}