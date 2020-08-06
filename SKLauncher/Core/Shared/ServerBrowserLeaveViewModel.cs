using System.Windows;

namespace Launcher.Core.Shared
{
    public class ServerBrowserLeaveViewModel : DependencyObject
    {
        private readonly LauncherSettings _settingsInstance;

        public ServerBrowserLeaveViewModel(LauncherSettings instance)
        {
            _settingsInstance = instance;
        }

        public bool DisableAsk
        {
            get => (bool)GetValue(DisableAskProperty);
            set => SetValue(DisableAskProperty, value);
        }
        public static readonly DependencyProperty DisableAskProperty =
            DependencyProperty.Register("DisableAsk", typeof(bool), typeof(ServerBrowserLeaveViewModel), new PropertyMetadata(false, _DisableAskChangedHandler));

        private static void _DisableAskChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = (ServerBrowserLeaveViewModel) d;
            var value = (bool) e.NewValue;

            vm._settingsInstance.DisableAskServerBrowserDiscordLeave = value;
        }
    }
}