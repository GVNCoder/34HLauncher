using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Shared;

using IBlurredPage = Launcher.Core.Bases.IBlurredPage;
using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.ViewModel
{
    public class EventLogViewModel : BasePageViewModel, IBlurredPage
    {
        private readonly IPageNavigator _navigator;
        private readonly IDiscord _discord;
        private readonly IUIHostService _hostService;
        private readonly IApplicationState _state;

        public ObservableCollection<EventViewModel> Events { get; }
        public Grid BackgroundContent { get; private set; }

        public EventLogViewModel(
            IUIHostService hostService,
            IDiscord discord,
            IPageNavigator navigator,
            IApplicationState state)
        {
            _navigator = navigator;
            _discord = discord;
            _hostService = hostService;
            _state = state;

            Events = new ObservableCollection<EventViewModel>();
            state.Application.MainWindow.Loaded += _windowLoadedHandler;
        }

        private void _windowLoadedHandler(object sender, RoutedEventArgs e)
        {
            BackgroundContent = _hostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;
            _state.Application.MainWindow.Loaded -= _windowLoadedHandler;
        }

        public ICommand CleanupEventsCommand => new DelegateCommand(obj =>
        {
            Events.Clear();

            _navigator.NavigateBack();
        });

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            _discord.UpdateAFK();
        });

        public override ICommand UnloadedCommand => null;
    }
}