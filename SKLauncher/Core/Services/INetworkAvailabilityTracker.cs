using System;
using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public interface INetworkAvailabilityTracker
    {
        bool CurrentConnectionState { get; }
        event EventHandler<NetworkEventArgs> AvailabilityChanged;
    }
}