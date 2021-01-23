using System;
using System.Timers;

using DiscordRPC;
using DiscordRPC.Message;
using log4net;

using Launcher.Core.Services;
using Launcher.Core.Shared;

using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.RPC
{
    public class Discord : IDiscord
    {
        #region Templates

        private readonly RichPresence _AFKRichPresence = new RichPresence
        {
            Details = "AFK",
            Assets = new Assets
            {
                LargeImageKey = "default_logo",
                SmallImageKey = "small_logo",
                SmallImageText = "ZLOEmu"
            }
        };

        private readonly RichPresence _ServerBrowserRichPresence = new RichPresence
        {
            Details = "Browse servers",
            Assets = new Assets
            {
                SmallImageKey = "small_logo",
                SmallImageText = "ZLOEmu"
            }
        };

        private readonly RichPresence _CoopBrowserRichPresence = new RichPresence
        {
            Details = "Browse COOP missions",
            Assets = new Assets
            {
                LargeImageKey = "bf3_large_logo",
                LargeImageText = "Battlefield 3",
                SmallImageKey = "small_logo",
                SmallImageText = "ZLOEmu"
            }
        };

        private readonly RichPresence _StatsRichPresence = new RichPresence
        {
            Details = "View statistics",
            Assets = new Assets
            {
                SmallImageKey = "small_logo",
                SmallImageText = "ZLOEmu"
            }
        };

        private readonly RichPresence _ServerRichPresence = new RichPresence
        {
            State = "In Game",
            Assets = new Assets
            {
                SmallImageKey = "small_logo",
                SmallImageText = "ZLOEmu"
            }
        };

        private readonly RichPresence _SingleRichPresence = new RichPresence
        {
            State = "In Game",
            Assets = new Assets
            {
                SmallImageKey = "small_logo",
                SmallImageText = "ZLOEmu"
            }
        };

        private readonly RichPresence _CoopRichPresence = new RichPresence
        {
            Details = "Cooperative",
            State = "In Game",
            Party = new Party
            {
                ID = Secrets.CreateFriendlySecret(new Random()),
                Max = 2,
                Size = 2
            },
            Assets = new Assets
            {
                SmallImageKey = "small_logo",
                SmallImageText = "ZLOEmu"
            }
        };

        #endregion

        private const string APP_TOKEN = "726427142019350578";
        private const int UPDATE_INTERVAL = 3000;

        private readonly string[] _BFTitles =
        {
            "Battlefield 3",
            "Battlefield 4",
            "Battlefield Hardline"
        };

        private readonly Timer _updateTimer;
        private readonly ILog _log;

        private DiscordRpcClient _rpcClient;
        private RichPresence _pagePresence;
        private RichPresence _gamePresence;

        private bool _gamePresenceToggle;

        public Discord(ILog log)
        {
            _log = log;

            // create update presence timer
            _updateTimer = new Timer
            {
                Enabled = false,
                AutoReset = true,
                Interval = UPDATE_INTERVAL
            };

            _updateTimer.Elapsed += _UpdateTimerCallback;

            // set default presence
            _pagePresence = _AFKRichPresence;
        }

        public void Start()
        {
            // create a new client instance
            _rpcClient = new DiscordRpcClient(APP_TOKEN, autoEvents: true);

            // subscribe and track events
            _rpcClient.OnReady += _RpcClientOnReadyHandler;  // Called when the client is ready to send presences
            _rpcClient.OnClose += _RpcClientOnCloseHandler;  // Called when connection to discord is lost
            _rpcClient.OnError += _RpcClientOnErrorHandler;  // Called when discord has a error
            _rpcClient.OnConnectionFailed += _RpcClientOnConnectionFailedHandler;  // Called when a pipe connection failed

            // start client
            _rpcClient.Initialize();
        }

        public void Stop()
        {
            _rpcClient.OnReady -= _RpcClientOnReadyHandler;  // Called when the client is ready to send presences
            _rpcClient.OnClose -= _RpcClientOnCloseHandler;  // Called when connection to discord is lost
            _rpcClient.OnError -= _RpcClientOnErrorHandler;  // Called when discord has a error
            _rpcClient.OnConnectionFailed -= _RpcClientOnConnectionFailedHandler;  // Called when a pipe connection failed

            // destroy client
            _rpcClient.ClearPresence();
            _rpcClient.Dispose();
            _rpcClient = null;
        }

        public void ToggleGame()
        {
            _gamePresenceToggle = false;
        }

        #region Page presence

        public void UpdateServerBrowser(ZGame game)
        {
            var title = _GetTitleByGame(game);

            _ServerBrowserRichPresence.Assets.LargeImageText = title;
            _ServerBrowserRichPresence.Assets.LargeImageKey = _GetLargeImageKeyByGame(game);

            _pagePresence = _ServerBrowserRichPresence;
        }

        public void UpdateCoopBrowser()
        {
            _pagePresence = _CoopBrowserRichPresence;
        }

        public void UpdateStats(ZGame game)
        {
            var title = _GetTitleByGame(game);

            _StatsRichPresence.Assets.LargeImageText = title;
            _StatsRichPresence.Assets.LargeImageKey = _GetLargeImageKeyByGame(game);

            _pagePresence = _StatsRichPresence;
        }

        public void UpdateAFK()
        {
            _pagePresence = _AFKRichPresence;
        }

        #endregion

        #region Game prensence

        public void UpdateServer(ZServerBase server)
        {
            _ServerRichPresence.Assets.LargeImageKey = _GetLargeImageKeyByGame(server.Game);
            _ServerRichPresence.Assets.LargeImageText = _GetTitleByGame(server.Game);
            _ServerRichPresence.Details = server.Name;
            _ServerRichPresence.Timestamps = Timestamps.Now;

            _gamePresence = _ServerRichPresence;
            _gamePresenceToggle = true;
        }

        public void UpdateCoop(ZPlayMode mode, CoopMissionModel model)
        {
            _CoopRichPresence.Assets.LargeImageKey = _GetLargeImageKeyByGame(ZGame.BF3);
            _CoopRichPresence.Assets.LargeImageText = mode == ZPlayMode.CooperativeHost
                ? _CoopRichPresence.Assets.LargeImageText = $"Host | {model.Name} | {model.Difficulty}"
                : _CoopRichPresence.Assets.LargeImageText = "In the lobby with my friend";
            _CoopRichPresence.Timestamps = Timestamps.Now;

            _gamePresence = _CoopRichPresence;
            _gamePresenceToggle = true;
        }

        public void UpdateSingle(ZGame game, ZPlayMode mode)
        {
            _SingleRichPresence.Assets.LargeImageKey = _GetLargeImageKeyByGame(game);
            _SingleRichPresence.Assets.LargeImageText = _GetTitleByGame(game);
            _SingleRichPresence.Details = mode == ZPlayMode.Singleplayer ? "Singleplayer" : "Playground";
            _SingleRichPresence.Timestamps = Timestamps.Now;

            _gamePresence = _SingleRichPresence;
            _gamePresenceToggle = true;
        }

        #endregion

        #region Private helpers

        private void _UpdateTimerCallback(object sender, ElapsedEventArgs e)
        {
            _rpcClient.SetPresence(_gamePresenceToggle ? _gamePresence : _pagePresence);
        }

        private string _GetTitleByGame(ZGame game) => _BFTitles[(int)game];

        private static string _GetLargeImageKeyByGame(ZGame game) => $"{game.ToString().ToLower()}_large_logo";

        private void _RpcClientOnConnectionFailedHandler(object sender, ConnectionFailedMessage args)
        {
            // restart
            Stop();
            Start();
        }

        private void _RpcClientOnErrorHandler(object sender, ErrorMessage args) =>
            _log.Warn($"DISCORD ERROR | {args.Code} {args.Message}");

        private void _RpcClientOnCloseHandler(object sender, CloseMessage args)
        {
            // log this shitty event
            _log.Warn($"DISCORD CONNECTION_LOST | {args.Code} {args.Reason}");

            // restart
            Stop();
            Start();
        }

        private void _RpcClientOnReadyHandler(object sender, ReadyMessage args)
        {
            // send first presence
            _rpcClient.SetPresence(_gamePresenceToggle ? _gamePresence : _pagePresence);

            // enable updates timer
            _updateTimer.Enabled = true;
        }

        #endregion
    }
}