using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Launcher.Core.Data;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Services.Updates;
using Launcher.Core.Shared;
using Launcher.ViewModel.UserControl;
using Ninject;
using Zlo4NET.Api;
using Zlo4NET.Api.DTO;
using Zlo4NET.Api.Models.Shared;

using Clipboard = Launcher.Helpers.Clipboard;
using WLM = Launcher.Localization.Loc.inCodeLocalizationMap.WindowViewLocalizationMap;
using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.ViewModel
{
    public class WindowNonClientPartViewModel : BaseViewModel
    {
        private readonly Window _wnd;
        private readonly IZApi _api;
        private readonly IMainMenuService _mainMenuService;
        private readonly ILauncherProcessService _launcherProcessService;
        private readonly IUpdateService _updateService;
        private readonly IPageNavigator _navigator;

        private ZUserDTO _authorizedUser;

        public WindowNonClientPartViewModel(
            IMainMenuService mainMenuService,
            IPageNavigator navigator,
            IZApi api,
            IUpdateService updateService)
        {
            UserPresenterViewModel = Resolver.Kernel.Get<UserPresenterViewModel>();
            UserPresenterViewModel.SetUserData(null);

            _navigator = navigator;

            _navigator.NavigationInitiated += _navigationInitiatedHandler;
            var application = Application.Current as App;

            _wnd = application.MainWindow;
            _launcherProcessService = application.ProcessService;
            _mainMenuService = mainMenuService;
            _api = api;
            _updateService = updateService;
        }

        private void _navigationInitiatedHandler(object sender, EventArgs e)
        {
            if (_mainMenuService.CanUse) _mainMenuService.Close();
        }

        private void _handler(object sender, ZConnectionChangedEventArgs e)
        {
            _api.Connection.ConnectionChanged -= _handler;
            Dispatcher.Invoke(() => ConnectIsEnabled = true);
        }

        #region Public members

        public UserPresenterViewModel UserPresenterViewModel { get; }

        public Visibility ConnectButtonVisibility
        {
            get => (Visibility)GetValue(ConnectButtonVisibilityProperty);
            set => Dispatcher.Invoke(() => SetValue(ConnectButtonVisibilityProperty, value));
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
            set => Dispatcher.Invoke(() => SetValue(CanBackNavigationProperty, value));
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

        #region Public methods

        public void UpdateDisconnected()
        {
            if (_authorizedUser == null) return;

            _authorizedUser = null;

            UserPresenterViewModel.SetUserData(null);
            CanBackNavigation = false;
            ConnectButtonVisibility = Visibility.Visible;
        }

        public void UpdateConnected()
        {
            if (_authorizedUser != null) return;

            _authorizedUser = _api.Connection.GetCurrentUserInfo();

            UserPresenterViewModel.SetUserData(_authorizedUser);
            CanBackNavigation = true;
            ConnectButtonVisibility = Visibility.Collapsed;
        }

        #endregion
    }
}