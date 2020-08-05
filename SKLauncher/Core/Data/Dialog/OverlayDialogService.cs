using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Launcher.Core.Services;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Shared;
using Launcher.XamlThemes.Controls;

namespace Launcher.Core.Data.Dialog
{
    public class OverlayDialogService : IOverlayDialogService
    {
        private readonly Dispatcher _dispatcher;
        private readonly UIHostService _hostService;
        private Panel _overlayContainer;

        public OverlayDialogService(UIHostService hostService)
        {
            _hostService = hostService;
            // wait for load window for HOST initialization
            Application.Current.MainWindow.Loaded += _mWndLoadedHandler;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        private void _mWndLoadedHandler(object sender, RoutedEventArgs e)
        {
            var wnd = (Window) sender;
            wnd.Loaded -= _mWndLoadedHandler;

            _overlayContainer = _hostService.GetHostContainer(UIElementConstants.HostOverlayContainer);
        }

        private void _UIInjector(bool injectionMode, UIElement element)
        {
            if (injectionMode) _overlayContainer.Children.Add(element);
            else _overlayContainer.Children.Remove(element);
        }

        public async Task<DialogResult> CreateDialog<TDialogControl>(BaseDialogViewModel dialogViewModel)
            where TDialogControl : UserControl, new()
        {
            var uiControl = _dispatcher.Invoke<OverlayControl>(() => new OverlayControl());
            var dialogAwaiter = new TaskCompletionSource<DialogResult>();
            dialogViewModel.Dialog = new DialogControlHelper(uiControl, dialogAwaiter);
            var dialogControl = new TDialogControl { DataContext = dialogViewModel };
            
            uiControl.Closed += (sender, e) => _UIInjector(false, uiControl);
            _UIInjector(true, uiControl);
            uiControl.Show(dialogControl);

            var result = await dialogAwaiter.Task;
            return result;
        }
    }
}