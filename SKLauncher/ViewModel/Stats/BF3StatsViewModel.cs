using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Launcher.Core.Bases;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;

using Clipboard = Launcher.Helpers.Clipboard;
using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.ViewModel.Stats
{
    public class BF3StatsViewModel : BasePageViewModel
    {
        private readonly IZApi _api;
        private readonly IApplicationState _state;
        private readonly IDiscord _discord;

        public BF3StatsViewModel(IUIHostService hostService, IZApi api, IDiscord discord, IApplicationState state)
        {
            _api = api;
            _state = state;
            _discord = discord;

            WindowBackgroundContent = hostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;
        }

        public ICommand OpenStatsCommand => new DelegateCommand(_OpenStatsExec);

        private void _OpenStatsExec(object obj)
        {
            Process.Start("https://bf3.zloemu.net/stats");
            Clipboard.CopyToClipboard(_api.Connection.GetCurrentUserInfo().UserName);
        }

        public override ICommand LoadedCommand => new DelegateCommand(_LoadedExec);
        public override ICommand UnloadedCommand => null;

        private void _LoadedExec(object obj)
        {
            //Stats = (ZBF3Stats) State.Storage["stats_BF3"];
            Stats = _state.GetState<ZBF3Stats>(Constants.BF3_STATS);

            _discord.UpdateStats(ZGame.BF3);
        }

        public Grid WindowBackgroundContent { get; }

        public ZBF3Stats Stats
        {
            get => (ZBF3Stats)GetValue(StatsProperty);
            set => SetValue(StatsProperty, value);
        }
        public static readonly DependencyProperty StatsProperty =
            DependencyProperty.Register("Stats", typeof(ZBF3Stats), typeof(BF3StatsViewModel), new PropertyMetadata(null));
    }
}