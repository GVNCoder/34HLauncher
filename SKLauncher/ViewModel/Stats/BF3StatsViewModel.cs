﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Launcher.Core.Bases;
using Launcher.Core.Interaction;
using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Shared;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;

using Clipboard = Launcher.Helpers.Clipboard;

namespace Launcher.ViewModel.Stats
{
    public class BF3StatsViewModel : PageViewModelBase
    {
        private readonly IZApi _api;

        public BF3StatsViewModel(IUIHostService hostService, IZApi api, IDiscordManager discord) : base(discord)
        {
            _api = api;
            WindowBackgroundContent = hostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;
        }

        public ICommand OpenStatsCommand => new DelegateCommand(_OpenStatsExec);

        private void _OpenStatsExec(object obj)
        {
            Process.Start("https://bf3.zloemu.net/stats");
            Clipboard.CopyToClipboard(_api.Connection.AuthorizedUser.Name);
        }

        public override ICommand LoadedCommand => new DelegateCommand(_LoadedExec);
        public override ICommand UnloadedCommand => null;

        private void _LoadedExec(object obj)
        {
            base.OnLoadedImpl();

            Stats = (ZBF3Stats) State.Storage["stats_BF3"];
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