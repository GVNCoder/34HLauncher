using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Launcher.Core.Bases;
using Launcher.Core.Interaction;
using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Shared;

namespace Launcher.ViewModel
{
    public class EventLogViewModel : PageViewModelBase, IBlurredPage
    {
        private readonly IUIHostService _hostService;
        private readonly IWindowContentNavigationService _navigationService;

        public ObservableCollection<EventViewModel> Events { get; }
        public Grid BackgroundContent { get; private set; }

        public EventLogViewModel(
            IUIHostService hostService,
            IWindowContentNavigationService navigationService,
            IDiscord discord) : base(discord)
        {
            _hostService = hostService;
            _navigationService = navigationService;

            var wnd = Application.Current.MainWindow;
            wnd.Loaded += _wndLoadedHandler;

            Events = new ObservableCollection<EventViewModel>();
        }

        private void _wndLoadedHandler(object sender, RoutedEventArgs e)
        {
            var wnd = (Window) sender;
            wnd.Loaded -= _wndLoadedHandler;

            BackgroundContent = _hostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;
        }

        public ICommand CleanupEventsCommand => new DelegateCommand(obj =>
        {
            Events.Clear();
            _navigationService.GoBack();
        });

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            _discord.UpdateAFK();
        });

        public override ICommand UnloadedCommand => null;
    }
}