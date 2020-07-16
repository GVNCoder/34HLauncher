using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Launcher.Core.Interaction;
using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Launcher.Styles.MapAndPlayerRotation;
using Launcher.UserControls;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

namespace Launcher.Core.Bases
{
    public abstract class BaseServerBrowserViewModel : PageViewModelBase, IBlurredPage
    {
        #region Props

        public string[] MapNames { get; protected set; }
        public string[] GameModeNames { get; protected set; }
        public Grid BackgroundContent { get; }

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

        protected BaseServerBrowserViewModel(
            IZApi api,
            IUIHostService uiHostService,
            IGameService gameService,
            IEventLogService eventLogService,
            IContentPresenterService modalContentPresenterService,
            IDiscord discord,
            Application application) : base(discord)
        {
            _api = api;
            _gameService = gameService;
            _eventLogService = eventLogService;
            _modalContentService = modalContentPresenterService;
            _application = application;

            BackgroundContent = (Grid) uiHostService.GetHostContainer(UIElementConstants.HostWindowBackground);
        }

        #region Protected members

        protected readonly IZApi _api;
        protected readonly IGameService _gameService;
        protected readonly Application _application;
        protected readonly IEventLogService _eventLogService;
        protected readonly IContentPresenterService _modalContentService;
        
        protected IZServersListService _serversService;
        protected CollectionViewSource _collectionViewSource;
        protected CollectionViewFiltrationExtension<ZServerBase> _viewFiltration;

        #endregion

        #region Protected methods

        protected CollectionViewSource _ExtractCollectionViewSource(Page ui)
        {
            return (CollectionViewSource) ui.Resources["CollectionViewSource"];
        }

        protected void _AssignCollectionViewSource(object source)
        {
            _collectionViewSource.Source = source;
        }

        protected void _JoinGame(ZServerBase server, ZRole role)
        {
            var context = new RunContext { Mode = ZPlayMode.Multiplayer, Target = server.Game, ServerId = server.Id, Role = role, Server = server };
            _gameService.Run(context);
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

        protected void OnLoadImpl(ZGame game, Page ui)
        {
            _collectionViewSource = _ExtractCollectionViewSource(ui);
            _serversService = _api.CreateServersListService(game);
            _serversService.InitialSizeReached += _serverListInitialSizeReached;

            _BuildViewFiltration(_collectionViewSource);
            _AssignCollectionViewSource(_serversService.ServersCollection);
            _serversService.StartReceiving();
        }

        protected void OnUnloadedImpl()
        {
            _serversService.InitialSizeReached -= _serverListInitialSizeReached;
            _serversService.ServersCollection.CollectionChanged -= _collectionChangedHandler;
            _serversService.StopReceiving();

            _ResetFilter(false);

            _collectionViewSource.Source = null;
            _collectionViewSource = null;
            _viewFiltration = null;

            SelectedServer = null;
        }

        protected void OnJoinImpl(ZRole role)
        {
            if (SelectedServer == null) return;
            if (SelectedServer.PlayersCapacity == SelectedServer.CurrentPlayersNumber) _eventLogService.Log(EventLogLevel.Warning, "Cannot join", "No slots available");
            _JoinGame(SelectedServer, role);
        }

        #endregion

        #region Private methods

        private async void _collectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            await ServerPingCalculator.CalculateAsync(_serversService.ServersCollection);
        }

        private void _serverListInitialSizeReached(object sender, EventArgs e)
        {
            _serversService.InitialSizeReached -= _serverListInitialSizeReached;
            _serversService.ServersCollection.CollectionChanged += _collectionChangedHandler;

            _collectionChangedHandler(null, null);
        }

        #endregion

        #region Commands

        public ICommand ShowRotationsCommand => new DelegateCommand(async obj =>
        {
            var players = SelectedServer.Players;
            var maps = SelectedServer.MapRotation.Rotation;

            var viewModel = new RotationsViewModel
            {
                Players = players,
                PlayersVisibility = players.Count == 0 ? Visibility.Visible : Visibility.Collapsed,
                Maps = maps,

                PlayerStyleSelector = new PlayerListViewStyleSelector(_application.Resources),
                MapStyleSelector = new MapListViewStyleSelector(_application.Resources)
            };

            await _modalContentService.Show<RotationsControl>(viewModel);
        });

        public ICommand ResetFilterCommand => new DelegateCommand(obj => _ResetFilter(true));

        public ICommand JoinCommand => new DelegateCommand(obj => OnJoinImpl((ZRole) obj));

        #endregion
    }
}