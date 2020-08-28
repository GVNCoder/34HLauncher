using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Services;
using Launcher.Core.Services.Updates;
using Launcher.Core.Shared;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;

using Clipboard = Launcher.Helpers.Clipboard;
using WLM = Launcher.Localization.Loc.inCodeLocalizationMap.WindowViewLocalizationMap;
using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.ViewModel.MainWindow
{
    public class WindowNonClientPartViewModel : DependencyObject
    {
        private readonly Window _wnd;
        private readonly IZApi _api;
        private readonly IMainMenuService _mainMenuService;
        private readonly IApplicationStateService _appStateService;
        private readonly ILauncherProcessService _launcherProcessService;
        private readonly IUpdateService _updateService;

        private readonly IPageNavigator _navigator;

        private ZUser _authorizedUser;

        public WindowNonClientPartViewModel(IUIHostService uiHostService,
            IApplicationStateService appStateService,
            IMainMenuService mainMenuService,
            //IWindowContentNavigationService navigationService,
            IPageNavigator navigator,
            IZApi api,
            IUpdateService updateService)
        {
            _navigator = navigator;

            WindowBackgroundContent = uiHostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;
            //Navigation = navigationService;
            _navigator.NavigationInitiated += _navigationInitiatedHandler;
            var application = Application.Current as App;

            _wnd = application.MainWindow;
            _appStateService = appStateService;
            _launcherProcessService = application.ProcessService;
            _mainMenuService = mainMenuService;
            _api = api;
            _updateService = updateService;

            _appStateService.StateChanged += _appStateChanged;
        }

        private void _navigationInitiatedHandler(object sender, EventArgs e)
        {
            if (_mainMenuService.CanUse) _mainMenuService.Close();
        }

        private void _SetAuthorizedUser()
        {
            _authorizedUser = _api.Connection.AuthorizedUser;
            if (_authorizedUser != null)
            {
                Dispatcher.Invoke(() =>
                {
                    UserName = _authorizedUser.Name;
                });
            }
        }

        private void _appStateChanged(object sender, ApplicationStateArgs e)
        {
            if (! e.State)
            {
                if (_authorizedUser == null)
                {
                    return;
                }

                Dispatcher.Invoke(() =>
                {
                    UserName = WLM.UnknownUser;
                    CanBackNavigation = false;
                    _authorizedUser = null;
                });
            }
            else if (_appStateService.AllGood())
            {
                if (_authorizedUser != null)
                {
                    return;
                }

                _SetAuthorizedUser();
                Dispatcher.Invoke(() => CanBackNavigation = true);
            }

            var visibility = _appStateService.AllGood() ? Visibility.Collapsed : Visibility.Visible;
            Dispatcher.Invoke(() => ConnectButtonVisibility = visibility);
        }

        private void _handler(object sender, ZConnectionChangedArgs e)
        {
            _api.Connection.ConnectionChanged -= _handler;
            Dispatcher.Invoke(() => ConnectIsEnabled = true);
        }

        #region Public members

        public Grid WindowBackgroundContent { get; }
        //public IWindowContentNavigationService Navigation { get; }

        public string UserName
        {
            get => (string)GetValue(UserNameProperty);
            set => Dispatcher.Invoke(() => SetValue(UserNameProperty, value));
        }
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(WindowNonClientPartViewModel), new PropertyMetadata(WLM.UnknownUser));

        public Visibility ConnectButtonVisibility
        {
            get => (Visibility)GetValue(ConnectButtonVisibilityProperty);
            set => SetValue(ConnectButtonVisibilityProperty, value);
        }
        public static readonly DependencyProperty ConnectButtonVisibilityProperty =
            DependencyProperty.Register("ConnectButtonVisibility", typeof(Visibility), typeof(WindowNonClientPartViewModel), new PropertyMetadata(Visibility.Visible));

        public bool ConnectIsEnabled
        {
            get => (bool)GetValue(ConnectIsEnabledProperty);
            set => SetValue(ConnectIsEnabledProperty, value);
        }
        public static readonly DependencyProperty ConnectIsEnabledProperty =
            DependencyProperty.Register("ConnectIsEnabled", typeof(bool), typeof(WindowNonClientPartViewModel), new PropertyMetadata(true));

        public bool CanBackNavigation
        {
            get => (bool)GetValue(CanBackNavigationProperty);
            set => SetValue(CanBackNavigationProperty, value);
        }
        public static readonly DependencyProperty CanBackNavigationProperty =
            DependencyProperty.Register("CanBackNavigation", typeof(bool), typeof(WindowNonClientPartViewModel), new PropertyMetadata(false));

        #endregion

        #region Commands

        public ICommand CloseWindowCommand => new DelegateCommand(obj => SystemCommands.CloseWindow(_wnd));

        public ICommand MinimizeWindowCommand => new DelegateCommand(obj => SystemCommands.MinimizeWindow(_wnd));

        public ICommand ToggleMainMenuCommand => new DelegateCommand(obj => _mainMenuService.Toggle());

        public ICommand NavigateToCommand => new DelegateCommand(obj =>
        {
            var navigationTarget = (string) obj;
            _navigator.Navigate(navigationTarget);
        });

        public ICommand NavigateBackCommand => new DelegateCommand(obj => _navigator.NavigateBack());

        public ICommand RestartApplicationCommand => new DelegateCommand(obj =>
        {
            if (!_updateService.InDownloadStage)
            {
                var location = AppDomain.CurrentDomain.BaseDirectory;
                var assemblyLocation = Path.Combine(location, "34H Launcher.exe");
                _launcherProcessService.RestartLauncher(Application.Current, assemblyLocation);
            }
            else
            {
                MessageBox.Show(WLM.DialogCannotRestart, WLM.CannotRestart, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        });

        public ICommand CopyUserIDCommand => new DelegateCommand(obj =>
        {
            if (_authorizedUser != null) Clipboard.CopyToClipboard($"{WLM.MyId} {_authorizedUser.Id}");
        });

        public ICommand ConnectCommand => new DelegateCommand(obj =>
        {
            // disable button
            ConnectIsEnabled = false;

            // try connect
            _api.Connection.Connect();
            _api.Connection.ConnectionChanged += _handler;
        });

        public ICommand DEBUG => new DelegateCommand(_DEBUG);

        private void _DEBUG(object obj)
        {
            if (_api.Connection.IsConnected) _api.Connection.Disconnect();
        }

        #endregion
    }
}