using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public class NetworkAvailabilityTracker : INetworkAvailabilityTracker
    {
        private EventHandler<NetworkEventArgs> handler;
        private readonly string[] _checkIgnoreInterfaces = new []
        {
            "Hamachi",
            "Radmin VPN",
        };

        private void DoNetworkAddressChanged(object sender, EventArgs e) => OnConnectionChangedHandler();
        private void DoNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e) => OnConnectionChangedHandler();

        public NetworkAvailabilityTracker()
        {
            CurrentConnectionState = _isNetworkAvailable();
        }

        private void OnConnectionChangedHandler()
        {
            // connection changed but, we don`t know what is mean. So handle this situation
            var newConnectionState = _isNetworkAvailable();
            var oldConnectionState = CurrentConnectionState;

            CurrentConnectionState = newConnectionState;
            OnNetworkAvailabilityChanged(oldConnectionState, newConnectionState);
        }

        private void OnNetworkAvailabilityChanged(bool oldConnectionState, bool newConnectionState)
        {
            if (oldConnectionState == newConnectionState) return;
            handler?.Invoke(this, new NetworkEventArgs(newConnectionState));
        }

        public event EventHandler<NetworkEventArgs> AvailabilityChanged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                if (handler == null)
                {
                    NetworkChange.NetworkAvailabilityChanged
                        += new NetworkAvailabilityChangedEventHandler(DoNetworkAvailabilityChanged);

                    NetworkChange.NetworkAddressChanged
                        += new NetworkAddressChangedEventHandler(DoNetworkAddressChanged);
                }

                handler = (EventHandler<NetworkEventArgs>)Delegate.Combine(handler, value);
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                handler = (EventHandler<NetworkEventArgs>)Delegate.Remove(handler, value);

                if (handler == null) return;
                
                NetworkChange.NetworkAvailabilityChanged
                    -= new NetworkAvailabilityChangedEventHandler(DoNetworkAvailabilityChanged);
                NetworkChange.NetworkAddressChanged
                    -= new NetworkAddressChangedEventHandler(DoNetworkAddressChanged);
            }
        }

        public bool CurrentConnectionState { get; private set; }
        
        private bool _isNetworkAvailable()
        {
            // only recognizes changes related to Internet adapters
            if (!NetworkInterface.GetIsNetworkAvailable()) return false;

            // however, this will include all adapters
            var interfaces = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(i => i.OperationalStatus == OperationalStatus.Up && i.NetworkInterfaceType != NetworkInterfaceType.Loopback && i.NetworkInterfaceType != NetworkInterfaceType.Tunnel);

            foreach (var face in interfaces)
            {
                if (_checkIgnoreInterfaces.Any(ignoreName => ignoreName.Equals(face.Name))) continue;

                IPv4InterfaceStatistics statistics = face.GetIPv4Statistics();

                // all testing seems to prove that once an interface comes online
                // it has already accrued statistics for both received and sent...

                if (statistics.BytesReceived > 0 && statistics.BytesSent > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}