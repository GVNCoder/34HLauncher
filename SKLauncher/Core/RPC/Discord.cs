using System;

using Launcher.Core.Shared;

using Ninject;
using DiscordRPC;

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

        private readonly RichPresence _CoopRichPresence = new RichPresence
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

        };

        #endregion

        private readonly string[] _BFTitles =
        {
            "Battlefield 3",
            "Battlefield 4",
            "Battlefield Hardline"
        };

        private readonly IDiscordPresence _discordPresence;

        private RichPresence _pagePresence;
        private RichPresence _gamePresence;
        private bool _useGamePresence;

        public Discord(App application)
        {
            _discordPresence = application.DependencyResolver.Resolver.Get<IDiscordPresence>();
            _discordPresence.ConnectionChanged += _discordConnectionChangedHandler;
        }

        private void _discordConnectionChangedHandler(object sender, DiscordConnectionChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                UpdateAFK();
            }
        }

        private string _GetTitleFullName(ZGame game) => _BFTitles[(int) game];

        public void UpdateServerBrowser(ZGame game)
        {
            var titleName = _GetTitleFullName(game);
            _ServerBrowserRichPresence.Assets.LargeImageText = titleName;
            _ServerBrowserRichPresence.Assets.LargeImageKey = $"{game.ToString().ToLower()}_large_logo";

            _updatePresence(_ServerBrowserRichPresence);
        }

        public void UpdateCoopBrowser()
        {
            _updatePresence(_CoopRichPresence);
        }

        public void UpdateStats(ZGame game)
        {
            switch (game)
            {
                case ZGame.BF3:
                    _StatsRichPresence.Assets.LargeImageText = "Battlefield 3";
                    break;
                case ZGame.BF4:
                    _StatsRichPresence.Assets.LargeImageText = "Battlefield 4";
                    break;
                case ZGame.BFH:
                case ZGame.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(game), game, null);
            }

            _StatsRichPresence.Assets.LargeImageKey = $"{game.ToString().ToLower()}_large_logo";
            _updatePresence(_StatsRichPresence);
        }

        public void UpdateAFK()
        {
            _updatePresence(_AFKRichPresence);
        }

        public void UpdateServer(ZServerBase server)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateCoop(ZPlayMode mode, CoopMissionModel model)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateSingle(ZGame game, ZPlayMode mode)
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            _discordPresence.BeginPresence();
        }

        public void Stop()
        {
            _discordPresence.StopPresence();
        }

        private void _updatePresence(RichPresence presence)
        {
            _discordPresence.UpdatePresence(presence);
        }
    }
}