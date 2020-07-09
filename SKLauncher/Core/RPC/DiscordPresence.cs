using System;
using System.Timers;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using log4net;

namespace Launcher.Core.RPC
{
    public class DiscordPresence : IDiscordPresence
    {
        private const string _token = "726427142019350578";
        private readonly Timer _updatePresenceTimer;
        private readonly ILog _logger;

        private RichPresence _lastPresence;
        private DiscordRpcClient _client;
        private bool _isReady;
        private bool _isInitialized;

        public DiscordPresence(ILog logger)
        {
            _logger = logger;
            _isReady = false;
            _isInitialized = false;

            _updatePresenceTimer = new Timer
            {
                Enabled = false,
                AutoReset = true,
                Interval = TimeSpan.FromSeconds(3).TotalMilliseconds
            };
            _updatePresenceTimer.Elapsed += _updatePresenceTimerElapsed;
        }

        public void BeginPresence()
        {
            if (_isInitialized) return;

            _client = _BuildRpcClient(_token);
            //_client.Logger = new ConsoleLogger(LogLevel.Error, true);
            _SubscribeHandlers(_client);
            _client.Initialize();
            _updatePresenceTimer.Start();
            _isInitialized = true;
        }

        public void StopPresence()
        {
            if (_isInitialized)
            {
                _updatePresenceTimer.Stop();
                _DestroyRpc();
                _isInitialized = false;
            }
        }

        public event EventHandler<DiscordConnectionChangedEventArgs> ConnectionChanged;

        private void _updatePresenceTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_isReady)
            {
                _client.SetPresence(_lastPresence);
            }
        }

        public void UpdatePresence(RichPresence presence)
        {
            _lastPresence = presence;
        }

        private void _OnConnectionChanged(bool connectionState)
            => ConnectionChanged?.Invoke(this, new DiscordConnectionChangedEventArgs(connectionState));

        private void _DestroyRpc()
        {
            _UnsubscribeHandlers(_client);
            _client.Dispose();
            _client = null;
        }

        private static DiscordRpcClient _BuildRpcClient(string token) => new DiscordRpcClient(token);

        private void _SubscribeHandlers(DiscordRpcClient rcpClient)
        {
            rcpClient.OnReady += _RpcClientOnReadyHandler;  // Called when the client is ready to send presences
            rcpClient.OnClose += _RpcClientOnCloseHandler;  // Called when connection to discord is lost
            rcpClient.OnError += _RpcClientOnErrorHandler;  // Called when discord has a error
            rcpClient.OnConnectionFailed += _RpcClientOnConnectionFailedHandler;  // Called when a pipe connection failed
        }

        private void _UnsubscribeHandlers(DiscordRpcClient rcpClient)
        {
            rcpClient.OnReady -= _RpcClientOnReadyHandler;  // Called when the client is ready to send presences
            rcpClient.OnClose -= _RpcClientOnCloseHandler;  // Called when connection to discord is lost
            rcpClient.OnError -= _RpcClientOnErrorHandler;  // Called when discord has a error
            rcpClient.OnConnectionFailed -= _RpcClientOnConnectionFailedHandler;  // Called when a pipe connection failed
        }

        private void _RpcClientOnConnectionFailedHandler(object sender, ConnectionFailedMessage args)
        {
            _RestartConnection();
        }

        private void _RpcClientOnErrorHandler(object sender, ErrorMessage args)
        {
            _logger.Warn($"DISCORD {args.Code} {args.Message}");
        }

        private void _RpcClientOnCloseHandler(object sender, CloseMessage args)
        {
            _RestartConnection();
        }

        private void _RpcClientOnReadyHandler(object sender, ReadyMessage args)
        {
            _isReady = true;
            _OnConnectionChanged(true);
        }

        private void _RestartConnection()
        {
            _isReady = false;
            _OnConnectionChanged(false);

            StopPresence();
            BeginPresence();
        }
    }
}