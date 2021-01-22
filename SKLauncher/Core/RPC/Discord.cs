using System;
using System.Linq;

using Ninject;

using DiscordRPC;

using Launcher.Core.Services;
using Launcher.Core.Shared;
using Launcher.Core.Data;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

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

        private readonly string[] _BFTitles =
        {
            "Battlefield 3",
            "Battlefield 4",
            "Battlefield Hardline"
        };

        private readonly IDiscordPresence _discordPresence;
        private readonly LauncherSettings _settings;
        private readonly IZConnection _zloConnection;

        private RichPresence _pagePresence;
        private RichPresence _gamePresence;
        private bool _useGamePresence;

        public Discord(IZApi zloApi, ISettingsService settingsService)
        {
            _zloConnection = zloApi.Connection;

            _discordPresence = Resolver.Kernel.Get<IDiscordPresence>();
            _discordPresence.ConnectionChanged += _discordConnectionChangedHandler;

            _settings = settingsService.GetLauncherSettings();
            _pagePresence = _AFKRichPresence;
        }

        private void _discordConnectionChangedHandler(object sender, DiscordConnectionChangedEventArgs e)
        {
            if (e.IsConnected) _updatePresence();
        }

        private string _GetTitleNameByGame(ZGame game) => _BFTitles[(int) game];

        private static string _GetLargeImageKeyByGame(ZGame game) => $"{game.ToString().ToLower()}_large_logo";

        public void UpdateServerBrowser(ZGame game)
        {
            var titleName = _GetTitleNameByGame(game);
            _ServerBrowserRichPresence.Assets.LargeImageText = titleName;
            _ServerBrowserRichPresence.Assets.LargeImageKey = _GetLargeImageKeyByGame(game);

            _pagePresence = _ServerBrowserRichPresence;
            _updatePresence();
        }

        public void UpdateCoopBrowser()
        {
            _pagePresence = _CoopBrowserRichPresence;
            _updatePresence();
        }

        public void UpdateStats(ZGame game)
        {
            var titleName = _GetTitleNameByGame(game);
            _StatsRichPresence.Assets.LargeImageText = titleName;
            _StatsRichPresence.Assets.LargeImageKey = _GetLargeImageKeyByGame(game);

            _pagePresence = _StatsRichPresence;
            _updatePresence();
        }

        public void UpdateAFK()
        {
            _pagePresence = _AFKRichPresence;
            _updatePresence();
        }

        public void UpdateServer(ZServerBase server)
        {
            _ServerRichPresence.Assets.LargeImageKey = _GetLargeImageKeyByGame(server.Game);
            _ServerRichPresence.Assets.LargeImageText = _zloConnection.AuthorizedUser?.Name;
            _ServerRichPresence.Details = server.Name;
            _ServerRichPresence.Timestamps = Timestamps.Now;

            _gamePresence = _ServerRichPresence;
            _useGamePresence = true;
            _updatePresence();
        }

        public void UpdateCoop(ZPlayMode mode, CoopMissionModel model)
        {
            _CoopRichPresence.Assets.LargeImageKey = _GetLargeImageKeyByGame(ZGame.BF3);
            _CoopRichPresence.Assets.LargeImageText = mode == ZPlayMode.CooperativeHost
                ? _CoopRichPresence.Assets.LargeImageText = $"Host | {model.Name} | {model.Difficulty}"
                : _CoopRichPresence.Assets.LargeImageText = $"In lobby on my friend";

            _CoopRichPresence.Timestamps = Timestamps.Now;

            _gamePresence = _CoopRichPresence;
            _useGamePresence = true;
            _updatePresence();
        }

        public void UpdateSingle(ZGame game, ZPlayMode mode)
        {
            _SingleRichPresence.Assets.LargeImageKey = _GetLargeImageKeyByGame(game);
            _SingleRichPresence.Assets.LargeImageText = _GetTitleNameByGame(game);
            _SingleRichPresence.Details = mode == ZPlayMode.Singleplayer ? "Singleplayer" : "Playground";
            _SingleRichPresence.Timestamps = Timestamps.Now;

            _gamePresence = _SingleRichPresence;
            _useGamePresence = true;
            _updatePresence();
        }

        public void Start()
        {
            _discordPresence.BeginPresence();
        }

        public void Stop()
        {
            _discordPresence.StopPresence();
        }

        public void DisablePlay()
        {
            _useGamePresence = false;
            _updatePresence();
        }

        private void _updatePresence()
        {
            if (_settings.UseDiscordPresence) _discordPresence.UpdatePresence(_useGamePresence ? _gamePresence : _pagePresence);
        }
    }
}