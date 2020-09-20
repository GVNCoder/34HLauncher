using System.Collections.Specialized;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Shared;
using Launcher.Data;

namespace Launcher.ViewModel.MainWindow
{
    public class WindowBottomBarPartViewModel : BaseViewModel
    {
        private readonly IEventLogService _eventLogService;
        private readonly IPageNavigator _navigator;

        private EventViewModel _disconnectedVm;

        public WindowBottomBarPartViewModel(
            IUIHostService uiHostService,
            IVersionService versionService,
            IEventLogService eventLogService,
            IPageNavigator navigator,
            IViewModelSource viewModelLocator)
        {
            _navigator = navigator;

            WindowBackgroundContent = uiHostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;
            VersionString = versionService.GetLauncherVersion().ToString();

            _eventLogService = eventLogService;

            var obsCollection = viewModelLocator.Get<EventLogViewModel>()
                .Events;
            obsCollection.CollectionChanged += _eventsCollectionChanged;
        }

        private void _eventsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (HasEvents == _eventLogService.HasEvents) return;
                HasEvents = _eventLogService.HasEvents;
            });
        }

        #region Public members

        public Grid WindowBackgroundContent { get; }
        public string VersionString { get; }

        public bool HasEvents
        {
            get => (bool)GetValue(HasEventsProperty);
            set => SetValue(HasEventsProperty, value);
        }
        public static readonly DependencyProperty HasEventsProperty =
            DependencyProperty.Register("HasEvents", typeof(bool), typeof(WindowBottomBarPartViewModel), new PropertyMetadata(false));

        #endregion

        #region Commands

        public ICommand OpenLogCommand => new DelegateCommand(_openLogCommandExec);

        private void _openLogCommandExec(object obj)
        {
            if (! _eventLogService.HasEvents) return;

            _navigator.Navigate("View/EventLogView.xaml");
        }

        #endregion

        #region Public methods

        public void UpdateConnected()
        {
            // check already call this method
            if (_disconnectedVm == null) return;

            // sync with UI thread
            Dispatcher.Invoke(() =>
            {
                _disconnectedVm?.CloseCommand.Execute(null);
                _disconnectedVm = null;
            });
        }

        public void UpdateDisconnected()
        {
            // check already call this method
            if (_disconnectedVm != null) return;

            _disconnectedVm = _eventLogService.Log(EventLogLevel.Error,
                "Unable to establish a connection to the ZClient for one of the following reasons:",
                "-ZClient not running\n-You did not click the Connect button\n-There is no internet connection\n" +
                "-You, for whatever reason, are not logged in to ZClient\n-Running multiple processes of ZClient\n" +
                "-Launcher internal error (restart the launcher and contact the developer)");
        }

        #endregion
    }
}