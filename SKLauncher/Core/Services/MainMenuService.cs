using System.Windows;
using System.Windows.Threading;
using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public class MainMenuService : IMainMenuService
    {
        private readonly IUIHostService _hostService;
        private readonly Dispatcher _dispatcher;

        private FrameworkElement _menuElement;

        public MainMenuService(IUIHostService hostService)
        {
            _hostService = hostService;
            _dispatcher = Dispatcher.CurrentDispatcher;
            var window = Application.Current.MainWindow;
            window.Loaded += _wndLoadedHandler;
        }

        private void _wndLoadedHandler(object sender, RoutedEventArgs e)
        {
            var wnd = (Window) sender;
            wnd.Loaded -= _wndLoadedHandler;

            _menuElement = _hostService.GetHostElement(UIElementConstants.HostMainMenu);
        }

        public void Close()
        {
            _dispatcher.Invoke(() => _menuElement.Visibility = Visibility.Collapsed);
        }

        public void Show()
        {
            _dispatcher.Invoke(() => _menuElement.Visibility = Visibility.Visible);
        }

        public void Toggle()
        {
            _dispatcher.Invoke(() =>
            {
                _menuElement.Visibility = _menuElement.Visibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            });
        }

        public bool CanUse => _menuElement != null;
    }
}