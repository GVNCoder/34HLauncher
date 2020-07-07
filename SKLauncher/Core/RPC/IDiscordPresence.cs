using System;
using DiscordRPC;

namespace Launcher.Core.RPC
{
    public interface IDiscordPresence
    {
        void BeginPresence();
        void StopPresence();
        void UpdatePresence(RichPresence presence);

        event EventHandler<DiscordConnectionChangedEventArgs> ConnectionChanged;
    }
}