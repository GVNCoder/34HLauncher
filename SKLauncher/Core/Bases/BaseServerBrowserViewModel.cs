﻿// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InvertIf

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Launcher.Core.Data;
using Launcher.Core.Dialog;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Launcher.Styles.MapAndPlayerRotation;
using Launcher.UserControls;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.Core.Bases
{
    public abstract class BaseServerBrowserViewModel : BasePageViewModel
    {
        #region Props

        public string[] MapNames { get; protected set; }
        public string[] GameModeNames { get; protected set; }

        #endregion

        #region Dependency props
        
        public ZServerBase SelectedServer
        {
            get => (ZServerBase)GetValue(SelectedServerProperty);
            set => SetValue(SelectedServerProperty, value);
        }
        public static readonly DependencyProperty SelectedServerProperty =
            DependencyProperty.Register("SelectedServer", typeof(ZServerBase), typeof(BaseServerBrowserViewModel), new PropertyMetadata(null));

        public int SelectedMapNameIndex
        {
            get => (int)GetValue(SelectedMapNameIndexProperty);
            set => SetValue(SelectedMapNameIndexProperty, value);
        }
        public static readonly DependencyProperty SelectedMapNameIndexProperty =
            DependencyProperty.Register("SelectedMapNameIndex", typeof(int), typeof(BaseServerBrowserViewModel), new PropertyMetadata(0, _MapNameSelectionChangedCallback));

        private static void _MapNameSelectionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = (BaseServerBrowserViewModel) d;
            var value = (int) e.NewValue;

            vm._viewFiltration[FilterConstant.MapName].Enable = vm.MapNames[value] != "All";
            vm._viewFiltration.UpdateView();
        }

        public int SelectedGameModeNameIndex
        {
            get => (int)GetValue(SelectedGameModeNameIndexProperty);
            set => SetValue(SelectedGameModeNameIndexProperty, value);
        }
        public static readonly DependencyProperty SelectedGameModeNameIndexProperty =
            DependencyProperty.Register("SelectedGameModeNameIndex", typeof(int), typeof(BaseServerBrowserViewModel), new PropertyMetadata(0, _GameModeNameSelectionChangedCallback));

        private static void _GameModeNameSelectionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = (BaseServerBrowserViewModel) d;
            var value = (int) e.NewValue;

            vm._viewFiltration[FilterConstant.GameModeName].Enable = vm.GameModeNames[value] != "All";
            vm._viewFiltration.UpdateView();
        }

        public bool ShowEmptyServers
        {
            get => (bool)GetValue(ShowEmptyServersProperty);
            set => SetValue(ShowEmptyServersProperty, value);
        }
        public static readonly DependencyProperty ShowEmptyServersProperty =
            DependencyProperty.Register("ShowEmptyServers", typeof(bool), typeof(BaseServerBrowserViewModel), new PropertyMetadata(false, _ShowEmptyServersChangedCallback));

        private static void _ShowEmptyServersChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = (BaseServerBrowserViewModel) d;
            var value = (bool) e.NewValue;

            if (value)
            {
                vm.ShowNotEmptyServers = false;
            }

            vm._viewFiltration[FilterConstant.Empty].Enable = value;
            vm._viewFiltration.UpdateView();
        }

        public bool ShowNotEmptyServers
        {
            get => (bool)GetValue(ShowNotEmptyServersProperty);
            set => SetValue(ShowNotEmptyServersProperty, value);
        }
        public static readonly DependencyProperty ShowNotEmptyServersProperty =
            DependencyProperty.Register("ShowNotEmptyServers", typeof(bool), typeof(BaseServerBrowserViewModel), new PropertyMetadata(false, _ShowNotEmptyServersChangedCallback));

        private static void _ShowNotEmptyServersChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = (BaseServerBrowserViewModel) d;
            var value = (bool) e.NewValue;

            if (value)
            {
                vm.ShowEmptyServers = false;
            }

            vm._viewFiltration[FilterConstant.NotEmpty].Enable = value;
            vm._viewFiltration.UpdateView();
        }

        #endregion

        #region Ctor

        protected BaseServerBrowserViewModel(
            IZApi api,
            IGameService gameService,
            IEventService eventService,
            IDiscord discord,
            Application application,
            ISettingsService settingsService,
            IPageNavigator navigator,
            IDialogService dialogService,
            IBusyIndicatorService busyIndicatorService)
        {
            _navigator = navigator;
            _discord = discord;

            _api = api;
            _gameService = gameService;
            _eventService = eventService;
            _application = application;
            _dialogService = dialogService;
            _settingsInstance = settingsService.GetLauncherSettings();
            _busyIndicatorService = busyIndicatorService;

            _serverListLoadTimeoutTimer = new Timer(TimeSpan.FromMilliseconds(800).TotalMilliseconds)
            {
                Enabled = false,
                AutoReset = false
            };

            _serverListLoadTimeoutTimer.Elapsed += (s, e)
                => Dispatcher.Invoke(() => _busyIndicatorService.Close());
        }

        #endregion

        #region Protected members

        protected readonly IPageNavigator _navigator;
        protected readonly IDialogService _dialogService;

        protected readonly IZApi _api;
        protected readonly IDiscord _discord;
        protected readonly IGameService _gameService;
        protected readonly Application _application;
        protected readonly IEventService _eventService;
        protected readonly LauncherSettings _settingsInstance;
        protected readonly IBusyIndicatorService _busyIndicatorService;
        
        protected IZServersListService _serversService;
        protected CollectionViewSource _collectionViewSource;
        protected CollectionViewFiltrationExtension<ZServerBase> _viewFiltration;
        protected ObservableCollection<ZServerBase> _internalServersCollection;

        private readonly Timer _serverListLoadTimeoutTimer;

        #endregion

        #region Protected methods

        protected void _JoinGame(ZServerBase server, ZRole role, string password)
        {
            // create run params
            var runParams = new CreateMultiplayerParameters
            {
                Game = server.Game,
                PlayerRole = role,
                ServerModel = server,
                ServerPassword = password
            };

            // run game
            _gameService.RunMultiplayer(runParams).Forget();
        }

        protected void _BuildViewFiltration(CollectionViewSource viewSource)
        {
            _viewFiltration = new CollectionViewFiltrationExtension<ZServerBase>(viewSource);
            
            _viewFiltration.AddFilter(FilterConstant.Empty, s => s.CurrentPlayersNumber == 0, false);
            _viewFiltration.AddFilter(FilterConstant.NotEmpty, s => s.CurrentPlayersNumber != 0, false);
            _viewFiltration.AddFilter(FilterConstant.MapName, s => s.MapRotation.Current.Name == MapNames[SelectedMapNameIndex], false);
            _viewFiltration.AddFilter(FilterConstant.GameModeName, s => s.MapRotation.Current.GameModeName == GameModeNames[SelectedGameModeNameIndex], false);

            _viewFiltration.Enabled = true;
        }

        protected void _ResetFilter(bool keepEnable)
        {
            _viewFiltration.Enabled = false;

            ShowEmptyServers = false;
            ShowNotEmptyServers = false;

            SelectedMapNameIndex = 0;
            SelectedGameModeNameIndex = 0;

            _viewFiltration.Enabled = keepEnable;
        }

        #endregion

        #region Command impl

        protected void OnLoadImpl(ZGame game, Page viewRef)
        {
            //_busyIndicatorService.Open();
            _collectionViewSource = (CollectionViewSource) viewRef.Resources["CollectionViewSource"];
            _serversService = _api.CreateServersListService(game);
            _internalServersCollection = new ObservableCollection<ZServerBase>();

            _BuildViewFiltration(_collectionViewSource);
            _collectionViewSource.Source = _internalServersCollection;

            _serversService.ServersCollection.CollectionChanged += (sender, args) =>
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:

                        // https://stackoverflow.com/questions/15281311/find-item-in-ilist-with-linq
                        var addedItem = args.NewItems.Cast<ZServerBase>()
                            .Single();
                        var serversCount = _internalServersCollection.Count;

                        if (serversCount == 0)
                        {
                            _internalServersCollection.Add(addedItem);
                        }
                        else
                        {
                            for (var i = 0; i < serversCount; i++)
                            {
                                var item = _internalServersCollection[i];

                                if (addedItem.Ping <= item.Ping)
                                {
                                    _internalServersCollection.Insert(i, addedItem);
                                    break;
                                }
                                
                                if (serversCount - 1 == i)
                                {
                                    _internalServersCollection.Add(addedItem);
                                }
                            }
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:

                        var removedItem = args.OldItems.Cast<ZServerBase>()
                            .Single();

                        _internalServersCollection.Remove(removedItem);

                        break;
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Move:
                    case NotifyCollectionChangedAction.Reset:
                    default:
                        return;
                }

                // https://stackoverflow.com/questions/1042312/how-to-reset-a-timer-in-c
                _serverListLoadTimeoutTimer.Stop();
                _serverListLoadTimeoutTimer.Start();
            };

            _serversService.StartReceiving();
            _busyIndicatorService.Open("Wait for server list loading...");
            _serverListLoadTimeoutTimer.Start();
            _discord.UpdateServerBrowser(game);
        }

        protected void OnUnloadedImpl()
        {
            _serversService.StopReceiving();

            _ResetFilter(false);

            _collectionViewSource.Source = null;
            _collectionViewSource = null;
            _viewFiltration = null;

            SelectedServer = null;
        }

        protected async Task OnJoinImpl(ZRole role)
        {
            if (SelectedServer == null)
            {
                return;
            }

            var serverAttributes = SelectedServer.Attributes;
            if (serverAttributes.ServerType == "PRIVATE")
            {
                var viewModel = new PasswordImputDialogViewModel("Private server", "Please, enter server password:");
                var dialogResult = await _dialogService.Show<DialogPasswordImputControl>(viewModel);

                if (dialogResult.Action == DialogAction.Primary)
                {
                    var serverPassword = dialogResult.GetResult<string>();
                    if (ZServerPassword.Verify(serverAttributes.ServerSecret, serverPassword))
                    {
                        _JoinGame(SelectedServer, role, serverPassword);
                    }
                    else
                    {
                        _eventService.ErrorEvent("Game run", "Invalid password");
                    }
                }
            }
            else
            {
                _JoinGame(SelectedServer, role, string.Empty);
            }
        }

        #endregion

        #region Private methods

        #endregion

        #region Commands

        public ICommand ShowRotationsCommand => new DelegateCommand(async obj =>
        {
            var viewModel =
                new RotationsViewModel(
                    SelectedServer.Players,
                    SelectedServer.MapRotation.Rotation);
            await _dialogService.OpenPresenter<DialogServerDetails>(viewModel);
        });

        public ICommand ResetFilterCommand => new DelegateCommand(obj => _ResetFilter(true));

        public ICommand JoinCommand => new DelegateCommand(obj => OnJoinImpl((ZRole) obj));

        #endregion
    }
}