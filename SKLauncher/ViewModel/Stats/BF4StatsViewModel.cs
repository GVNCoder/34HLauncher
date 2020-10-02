using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Zlo4NET.Api.Models.Shared;

using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.ViewModel.Stats
{
    public class BF4StatsViewModel : BasePageViewModel
    {
        private readonly IApplicationState _state;
        private readonly IDiscord _discord;

        public BF4StatsViewModel(IUIHostService hostService, IDiscord discord, IApplicationState state)
        {
            _state = state;
            _discord = discord;

            WindowBackgroundContent = hostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;
        }

        public override ICommand LoadedCommand => new DelegateCommand(_LoadedExec);
        public override ICommand UnloadedCommand => null;

        private void _LoadedExec(object obj)
        {
            Stats = _state.GetState<ZBF4Stats>(Constants.BF4_STATS);

            _discord.UpdateStats(ZGame.BF4);
        }

        public Grid WindowBackgroundContent { get; }

        public ZBF4Stats Stats
        {
            get => (ZBF4Stats) GetValue(StatsProperty);
            set => SetValue(StatsProperty, value);
        }
        public static readonly DependencyProperty StatsProperty =
            DependencyProperty.Register("Stats", typeof(ZBF4Stats), typeof(BF4StatsViewModel), new PropertyMetadata(null));
    }
}