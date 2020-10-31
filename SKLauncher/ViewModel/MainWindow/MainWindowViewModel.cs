using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using log4net;
using Microsoft.Win32;

using Ninject.Syntax;

using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Services;
using Launcher.Core.Services.Updates;
using Launcher.Core;
using Launcher.Core.Data;
using Launcher.Core.Dialog;
using Launcher.Core.Service.Base;
using Launcher.Helpers;
using Launcher.UserControls;
using Launcher.View;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;

using IDiscord = Launcher.Core.RPC.IDiscord;
using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.ViewModel
{
    public class MainWindowViewModel : BasePageViewModel
    {
        private const string _ZClientProcessName = "ZClient";

        private readonly ZProcessTracker _zClientProcessTracker;
        private readonly ISettingsService _settingsService;
        private readonly IProcessService _processService;
        private readonly IZConnection _apiConnection;
        private readonly ILog _log;
        private readonly IUpdateService _updateService;
        private readonly IZApi _api;

        private readonly IEventService _eventService;
        private readonly IMainMenuService _menuService;
        private readonly IGameService _gameService;
        private readonly IDiscord _discordPresence;

        private readonly IPageNavigator _navigator;
        private readonly IApplicationState _state;
        private readonly IDialogSystemBase _dialogSystemBase;
        private readonly IDialogService _dialogService;
        private readonly IBusyIndicatorBase _busyIndicatorBase;
        private readonly IBusyIndicatorService _busyIndicatorService;

        public MainWindowViewModel(
            IZApi api,
            ILog logger,
            ISettingsService settingsService,
            IProcessService processService,
            IResolutionRoot kernel,
            IEventService eventService,
            IMainMenuService menuService,
            IUpdateService updateService,
            IGameService gameService,
            IDiscord discordPresence,
            
            IPageNavigator navigator,
            IApplicationState state,
            IViewModelSource viewModelSource,
            IDialogService dialogService,
            IDialogSystemBase dialogSystemBase,
            IBusyIndicatorBase busyIndicatorBase,
            IBusyIndicatorService busyIndicatorService)
        {
            _navigator = navigator;
            _state = state;
            _dialogSystemBase = dialogSystemBase;
            _dialogService = dialogService;
            _busyIndicatorBase = busyIndicatorBase;
            _busyIndicatorService = busyIndicatorService;
            _eventService = eventService;

            _menuService = menuService;

            _apiConnection = api.Connection;
            _log = logger;
            _settingsService = settingsService;
            _processService = processService;
            _updateService = updateService;
            _gameService = gameService;
            _discordPresence = discordPresence;
            _api = api;

            NonClientDataContext = viewModelSource.Create<WindowNonClientPartViewModel>();
            BottomBarDataContext = viewModelSource.Create<WindowBottomBarPartViewModel>();

            _apiConnection.ConnectionChanged += _apiConnectionConnectionChangedHandler;

            _zClientProcessTracker = new ZProcessTracker(_ZClientProcessName, TimeSpan.FromSeconds(3), true, processes => processes
                .OrderByDescending(process => process.StartTime)
                .First());
            _zClientProcessTracker.ProcessDetected += _zClientProcessDetectedHandler;
            _zClientProcessTracker.ProcessLost += _zClientProcessLostHandler;

            api.Configure(new ZConfiguration { SynchronizationContext = SynchronizationContext.Current });

            _gameService.GameClose += _GameCloseHandler;
            _gameService.GameRunError += _GameRunErrorHandler;
        }

        private void _GameRunErrorHandler(object sender, GameRunErrorEventArgs e)
        {
            //_eventLogService.Log(EventLogLevel.Warning, SLM.GameRun, e.Error.Message);
            _log.Error(LoggingHelper.GetMessage(e.Error));
        }

        private void _GameCloseHandler(object sender, GameCloseEventArgs e)
        {
            //_eventLogService.Log(EventLogLevel.Message, SLM.GameRun, e.PipeLog);
        }

        private void _RefreshAppState()
        {
            var connectionState = _zClientProcessTracker.IsRun && _apiConnection.IsConnected;
            if (connectionState)
            {
                // update data contexts state
                NonClientDataContext.UpdateConnected();
                BottomBarDataContext.UpdateConnected();
            }
            else
            {
                // update data contexts state
                NonClientDataContext.UpdateDisconnected();
                BottomBarDataContext.UpdateDisconnected();

                // goto Home ;)
                _navigator.Navigate("View\\HomeView.xaml");
            }

            // update app state
            _state.SetState(Constants.ZCLIENT_CONNECTION, connectionState);
        }

        private void _zClientProcessLostHandler(object sender, EventArgs e)
        {
            _RefreshAppState();

            // reset connection
            _apiConnection.Disconnect();
        }

        private async void _zClientProcessDetectedHandler(object sender, Process process)
        {
            _RefreshAppState();

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
                        //_eventLogService.Log(EventLogLevel.Warning, "ZClient path", "ZClient not detected");
                    }
                }
                catch (Exception)
                {
                    //_eventLogService.Log(EventLogLevel.Error, "ZClient path", "An error occurred while trying to get the path to ZClient.");
                }
            }

            if (_settingsService.GetLauncherSettings().TryToConnect)
            {
                _apiConnection.Connect();
            }
        }

        private void _apiConnectionConnectionChangedHandler(object sender, ZConnectionChangedArgs e)
            => _RefreshAppState();

        private void _runZClient(string path)
        {
            var runResult = _processService.Run(path, true);
            if (!runResult)
            {
                //_eventLogService.Log(EventLogLevel.Warning, "Cannot run ZClient",
                //    "Wrong path or unknown error. Try again.");
            }
        }

        #region Public properties

        public WindowNonClientPartViewModel NonClientDataContext { get; }
        public WindowBottomBarPartViewModel BottomBarDataContext { get; }

        #endregion

        #region Commands

        public override ICommand LoadedCommand => new DelegateCommand(parameter =>
        {
            var windowInstance = (MainWindowView) parameter;

            // setup ui dependencies
            _navigator.SetDependency(windowInstance.HOST_Content);
            _dialogSystemBase.SetDependency(windowInstance.HOST_DialogContainer);
            _busyIndicatorBase.SetDependency(windowInstance.HOST_DialogContainer);

            //// TODO: Testing only
            //windowInstance.MouseEnter += (s, e) => _busyIndicatorBase.Open("Please wait...");
            //windowInstance.MouseLeave += (s, e) => _busyIndicatorBase.Close();

            // setup application state vars
            _state.RegisterVars();

            // initial actions
            _navigator.Navigate("View\\HomeView.xaml"); // default page is Home ;)
            BottomBarDataContext.UpdateDisconnected();  // for error message showing

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

            _state.SetState(Constants.ZCLIENT_IS_RUN, _zClientProcessTracker.IsRun);
            _state.SetState(Constants.ZCLIENT_CONNECTION, _apiConnection.IsConnected);
        });

        public override ICommand UnloadedCommand => new DelegateCommand(parameter =>
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
                    //_eventLogService.Log(EventLogLevel.Message, "Run ZClient", "ZClient already run.");
                }
                else
                {
                    _runZClient(launcherSettings.PathToZClient);
                }
            }
            else
            {
                await _dialogService.OpenTextDialog("Run ZClient",
                    "Path to ZClient.exe was not set. Please set it in Settings and try again.", DialogButtons.Ok);
            }
        });

        public ICommand ShowAboutCommand => new AsyncCommand(() =>
        {
            _menuService.Close();

            return _dialogService.OpenPresenter<UserAbout>(null);
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
