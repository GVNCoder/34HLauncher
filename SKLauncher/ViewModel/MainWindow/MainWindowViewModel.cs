using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using log4net;
using Launcher.Core.Data;
using Microsoft.Win32;
using Ninject;

using Launcher.Core.Data.Updates;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Services;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Services.Updates;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Launcher.UserControls;
using Launcher.View;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;
using IDiscord = Launcher.Core.RPC.IDiscord;
using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.ViewModel.MainWindow
{
    public class MainWindowViewModel : DependencyObject
    {
        private const string _ZClientProcessName = "ZClient";

        #region Lazy services

        private readonly Lazy<IWindowContentNavigationService> __lazyWindowNavigationService;
        private IWindowContentNavigationService _navigationService => __lazyWindowNavigationService.Value;

        #endregion

        private readonly IApplicationStateService _appStateService;
        private readonly ZProcessTracker _zClientProcessTracker;
        private readonly ISettingsService _settingsService;
        private readonly IProcessService _processService;
        private readonly IZConnection _apiConnection;
        private readonly ILog _log;
        private readonly IUpdateService _updateService;
        private readonly IZApi _api;

        private readonly ITextDialogService _textDialogService;
        private readonly IContentPresenterService _contentPresenterService;
        private readonly IEventLogService _eventLogService;
        private readonly IMainMenuService _menuService;
        private readonly IGameService _gameService;
        private readonly IDiscord _discordPresence;

        private readonly IPageNavigator _navigator;

        public MainWindowViewModel(
            IApplicationStateService appStateService,
            IZApi api,
            IUIHostService hostService,
            ILog logger,
            ISettingsService settingsService,
            IProcessService processService,
            IKernel kernel,
            ITextDialogService dialogService,
            IContentPresenterService contentPresenterService,
            IEventLogService eventLogService,
            IMainMenuService menuService,
            IUpdateService updateService,
            IGameService gameService,
            IDiscord discordPresence,
            
            IPageNavigator navigator)
        {
            _navigator = navigator;
            
            _textDialogService = dialogService;
            _contentPresenterService = contentPresenterService;
            _eventLogService = eventLogService;
            _menuService = menuService;

            _appStateService = appStateService;
            _apiConnection = api.Connection;
            _log = logger;
            _settingsService = settingsService;
            _processService = processService;
            _updateService = updateService;
            _gameService = gameService;
            _discordPresence = discordPresence;
            _api = api;

            NonClientDataContext = kernel.Get<WindowNonClientPartViewModel>();
            BottomBarDataContext = kernel.Get<WindowBottomBarPartViewModel>();

            _apiConnection.ConnectionChanged += _apiConnectionConnectionChangedHandler;

            _zClientProcessTracker = new ZProcessTracker(_ZClientProcessName, TimeSpan.FromSeconds(3), true, processes => processes
                .OrderByDescending(process => process.StartTime)
                .First());
            _zClientProcessTracker.ProcessDetected += _zClientProcessDetectedHandler;
            _zClientProcessTracker.ProcessLost += _zClientProcessLostHandler;

            _appStateService.StateChanged += _appStateChanged;

            __lazyWindowNavigationService = new Lazy<IWindowContentNavigationService>(() =>
                kernel.Get<IWindowContentNavigationService>());

            _updateService.CancelDownloadResolver = async () =>
            {
                var dlgResult = await _textDialogService.OpenDialog("Are you sure ?",
                    "Are you sure you want to stop downloading the update?", TextDialogButtons.Ok | TextDialogButtons.No);
                return dlgResult.Action == DialogActionEnum.Primary;
            };
            _updateService.UpdateAvailableResolver = ver =>
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                var dlgResult = MessageBox.Show(Application.Current.MainWindow, "Download and install it?", $"A new version is available {ver}",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                return dlgResult == MessageBoxResult.Yes;
            };
            _updateService.Error += _updateServiceErrorHandler;

            api.Configure(new ZConfiguration { SynchronizationContext = SynchronizationContext.Current });

            _gameService.GameClose += _GameCloseHandler;
            _gameService.GameRunError += _GameRunErrorHandler;
        }

        private void _GameRunErrorHandler(object sender, GameRunErrorEventArgs e)
        {
            _eventLogService.Log(EventLogLevel.Warning, SLM.GameRun, e.Error.Message);
            _log.Error(LoggingHelper.GetMessage(e.Error));
        }

        private void _GameCloseHandler(object sender, GameCloseEventArgs e)
        {
            _eventLogService.Log(EventLogLevel.Message, SLM.GameRun, e.PipeLog);
        }

        private void _updateServiceErrorHandler(object sender, UpdateErrorEventArgs e)
            => _eventLogService.Log(EventLogLevel.Error, "Update service", e.Message);

        private void _appStateChanged(object sender, ApplicationStateArgs e)
        {
            if (! e.State)
            {
                // _navigationService.NavigateTo("View\\HomeView.xaml");
            }

            State.Storage["connection"] = _appStateService.AllGood();
        }

        private void _zClientProcessLostHandler(object sender, EventArgs e)
        {
            _appStateService.ChangeState(StateConstants.ZClient, false);
            _appStateService.ChangeState(StateConstants.Monolith, false);

            _apiConnection.Disconnect();
        }

        private async void _zClientProcessDetectedHandler(object sender, Process process)
        {
            _appStateService.ChangeState(StateConstants.ZClient, true);

            var startProcessTime = process.StartTime.ToUniversalTime();
            var currentTime = DateTime.UtcNow;
            var diff = currentTime - startProcessTime;

            if (Math.Floor((decimal) diff.TotalSeconds) < 3m)
            {
                // wait 3.5 seconds for full ZClient start
                await Task.Delay(3500);
            }

            if (string.IsNullOrEmpty(_settingsService.GetLauncherSettings().PathToZClient))
            {
                try
                {
                    var path = (string) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\ZLO", "ZClientPath",
                        string.Empty);
                    if (!string.IsNullOrEmpty(path))
                    {
                        _settingsService.GetLauncherSettings().PathToZClient = Path.Combine(path, "ZClient.exe");
                    }
                    else
                    {
                        _eventLogService.Log(EventLogLevel.Warning, "ZClient path", "ZClient not detected");
                    }
                }
                catch (Exception)
                {
                    _eventLogService.Log(EventLogLevel.Error, "ZClient path", "An error occurred while trying to get the path to ZClient.");
                }
            }

            if (_settingsService.GetLauncherSettings().TryToConnect)
            {
                _apiConnection.Connect();
            }
        }

        private void _apiConnectionConnectionChangedHandler(object sender, ZConnectionChangedArgs e)
        {
            _appStateService.ChangeState(StateConstants.Monolith, e.IsConnected);
        }

        private void _runZClient(string path)
        {
            var runResult = _processService.Run(path, true);
            if (!runResult)
            {
                _eventLogService.Log(EventLogLevel.Warning, "Cannot run ZClient",
                    "Wrong path or unknown error. Try again.");
            }
        }

        #region Public properties

        public WindowNonClientPartViewModel NonClientDataContext { get; }
        public WindowBottomBarPartViewModel BottomBarDataContext { get; }

        #endregion

        #region Commands

        public ICommand WindowLoadedCommand => new DelegateCommand(_windowLoadedCommandExec);

        private void _windowLoadedCommandExec(object obj)
        {
            var iWnd = (MainWindowView) obj;

            _navigationService.Initialize(iWnd.HOST_Content);
            _navigationService.NavigateTo("View\\HomeView.xaml");

            // setup ui dependencies
            ((PageNavigator) _navigator).SetDependency(iWnd.HOST_Content);





            _navigator.Navigate("View\\HomeView.xaml");

            _appStateService.AddState(StateConstants.ZClient, true);
            _appStateService.AddState(StateConstants.Monolith, true);

            var settings = _settingsService.GetLauncherSettings();
            if (settings.RunZClient)
            {
                if (Process.GetProcessesByName(_ZClientProcessName).Length == 0)
                {
                    _runZClient(settings.PathToZClient);
                }
            }

            if (settings.UseDiscordPresence)
            {
                _discordPresence.Start();
            }

            _gameService.TryDetect();
            _updateService.BeginUpdate();
            _zClientProcessTracker.StartTrack();

            _appStateService.ChangeState(StateConstants.ZClient, _zClientProcessTracker.IsRun);
            _appStateService.ChangeState(StateConstants.Monolith, _apiConnection.IsConnected);
        }

        public ICommand UnloadedCommand => new DelegateCommand(obj =>
        {
            _discordPresence.Stop();
        });

        public ICommand OpenForumCommand => new DelegateCommand(obj =>
        {
            _menuService.Close();
            Process.Start("https://zlogames.ru/index.php?/forums/");
        });

        public ICommand OpenZLOEmuCommand => new DelegateCommand(obj =>
        {
            _menuService.Close();
            Process.Start("https://zloemu.net/");
        });

        public ICommand OpenLauncherPageCommand => new DelegateCommand(obj =>
        {
            _menuService.Close();
            Process.Start("https://zlogames.ru/index.php?/topic/11434/");
        });

        public ICommand RunZClientCommand => new DelegateCommand(async obj =>
        {
            _menuService.Close();
            var launcherSettings = _settingsService.GetLauncherSettings();
            if (!string.IsNullOrEmpty(launcherSettings.PathToZClient))
            {
                if (Process.GetProcessesByName(_ZClientProcessName).Length > 0)
                {
                    _eventLogService.Log(EventLogLevel.Message, "Run ZClient", "ZClient already run.");
                }
                else
                {
                    _runZClient(launcherSettings.PathToZClient);
                }
            }
            else
            {
                await _textDialogService.OpenDialog("Run ZClient",
                    "Path to ZClient.exe was not set. Please set it in Settings and try again.", TextDialogButtons.Ok);
            }
        });

        public ICommand ShowAboutCommand => new DelegateCommand(async obj =>
        {
            _menuService.Close();
            await _contentPresenterService.Show<UserAbout>(null);
        });

        public ICommand OnClosingCommand => new DelegateCommand(_OnClosingExec);

        private void _OnClosingExec(object obj)
        {
            if (!(obj is CancelEventArgs e)) return;

            if (_updateService.InDownloadStage)
            {
                var dlgResult = MessageBox.Show("Are you sure you want to stop downloading the update?",
                    "Are you sure ?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                e.Cancel = dlgResult != MessageBoxResult.Yes;
            }
            else
            {
                e.Cancel = false;
            }
        }

        #endregion
    }
}
