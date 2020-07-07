using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Launcher.Core.Interaction;
using Launcher.Core.Services;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Shared;

namespace Launcher.ViewModel.MainWindow
{
    public class WindowBottomBarPartViewModel : DependencyObject
    {
        private readonly IApplicationStateService _applicationStateService;
        private readonly IEventLogService _eventLogService;

        private EventViewModel _zClientEvent;
        private EventViewModel _connectionEvent;

        public WindowBottomBarPartViewModel(
            IUIHostService uiHostService,
            IVersionService versionService,
            IApplicationStateService applicationStateService,
            IWindowContentNavigationService navigationService,
            IEventLogService eventLogService,
            App application)
        {
            WindowBackgroundContent = uiHostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;
            VersionString = versionService.GetLauncherVersion().ToString();
            NavigationService = navigationService;

            _applicationStateService = applicationStateService;
            _applicationStateService.StateChanged += _applicationStateChangedHandler;
            _eventLogService = eventLogService;

            var vmLocator = application
                .DependencyResolver
                .Locators
                .ViewModelLocator;
            var obsCollection = vmLocator
                .EventLogViewModel
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

        private void _logZClientNotDetectedEvent()
        {
            _zClientEvent = _eventLogService.Log(EventLogLevel.Error, "Cannot detect ZClient. List of possible problems:",
                "-ZClient not running\n-Internal launcher error (restart the launcher and contact the developer)");
        }

        private void _logConnectionLostEvent()
        {
            _connectionEvent = _eventLogService.Log(EventLogLevel.Error, "Cannot create connection. List of possible problems:",
                "-You did not click the Connect button\n-There is no internet connection\n-You, for whatever reason, are not logged in to ZClient\n-Running multiple processes of ZClient\n-Launcher internal error (restart the launcher and contact the developer)");
        }

        private void _applicationStateChangedHandler(object sender, ApplicationStateArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Key == StateConstants.ZClient)
                {
                    if (e.State)
                    {
                        _zClientEvent?.CloseCommand.Execute(null);
                        _zClientEvent = null;
                    }
                    else
                    {
                        _logZClientNotDetectedEvent();
                    }
                }
                else
                {
                    if (e.State)
                    {
                        _connectionEvent?.CloseCommand.Execute(null);
                        _connectionEvent = null;
                    }
                    else
                    {
                        _logConnectionLostEvent();
                    }
                }
            });
        }

        #region Public members

        public Grid WindowBackgroundContent { get; }
        public string VersionString { get; }
        public IWindowContentNavigationService NavigationService { get; }

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
            if (!_eventLogService.HasEvents) return;
            NavigationService.NavigateTo("View/EventLogView.xaml");
        }

        #endregion
    }
}