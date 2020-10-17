using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Launcher.Core.Data;
using Launcher.Core.Dialog;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Launcher.UserControls;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;

using HLM = Launcher.Localization.Loc.inCodeLocalizationMap.HomeViewLocalizationMap;
using IDiscord = Launcher.Core.RPC.IDiscord;
using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.ViewModel
{
    public class HomeViewModel : BasePageViewModel
    {
        private readonly IPageNavigator _navigator;
        private readonly IApplicationState _state;

        private readonly IEventLogService _eventLogService;
        private readonly IGameService _gameService;
        private readonly ISettingsService _settingsService;
        private readonly IZApi _api;
        private readonly IBusyService _busyService;
        private readonly IDiscord _discord;
        private readonly IDialogService _dialogService;

        public HomeViewModel(
            IEventLogService eventLogService,
            IGameService gameService,
            ISettingsService settingsService,
            IZApi api,
            IBusyService busyService,
            IDiscord discord,
            IPageNavigator navigator,
            IApplicationState state,
            IDialogService dialogService)
        {
            _navigator = navigator;
            _state = state;
            _discord = discord;

            _eventLogService = eventLogService;
            _gameService = gameService;
            _settingsService = settingsService;
            _api = api;
            _busyService = busyService;
            _dialogService = dialogService;
        }

        private static bool _isOnline(object playMode)
        {
            switch (playMode)
            {
                case ZPlayMode.CooperativeHost:
                case ZPlayMode.CooperativeClient:
                case ZPlayMode.Multiplayer:
                    return true;
                case ZPlayMode.Singleplayer:
                case ZPlayMode.TestRange:
                default:
                    return false;
            }
        }

        #region Dependency properties

        public double CardTransparency
        {
            get => (double)GetValue(CardTransparencyProperty);
            set => SetValue(CardTransparencyProperty, value);
        }
        public static readonly DependencyProperty CardTransparencyProperty =
            DependencyProperty.Register("CardTransparency", typeof(double), typeof(HomeViewModel), new PropertyMetadata(.1d));

        #endregion

        #region Commands

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            var settings = _settingsService.GetLauncherSettings();
            CardTransparency = settings.CardTransparency;

            _discord.UpdateAFK();
        });

        public override ICommand UnloadedCommand => null;

        public ICommand OpenSettingsCommand => new DelegateCommand(_OpenSettingsExec);

        private void _OpenSettingsExec(object obj)
        {
            var targetGame = EnumUtil.Parse<ZGame>((string) obj);
            var settings = _settingsService.GetGameSettings().Settings[(int) targetGame];
            var viewModel = new GameSettingsViewModel(settings, _settingsService, targetGame != ZGame.BF3);

            _dialogService.OpenPresenter<GameSettingsControl>(viewModel).Forget();
        }

        public ICommand JoinGameCommand => new DelegateCommand(_JoinGameCommandExec);

        private void _JoinGameCommandExec(object obj)
        {
            // extract useful data
            var stringObj = (string) obj;
            var parameters = stringObj.Split('.');
            var game = EnumUtil.Parse<ZGame>(parameters.First());
            var playMode = EnumUtil.Parse<ZPlayMode>(parameters.Last());
            var connected = _state.GetState<bool>(Constants.ZCLIENT_CONNECTION);

            // check connection
            if (! connected)
            {
                _eventLogService.Log(EventLogLevel.Warning, SLM.GameRun,
                    _isOnline(playMode) ? HLM.EventLauncherDisconnectedServers : HLM.EventLauncherDisconnected);

                return;
            }

            if (_isOnline(playMode))
            {
                switch (playMode)
                {
                    case ZPlayMode.Multiplayer:
                        _navigator.Navigate($"View\\{game}MultiplayerView.xaml");
                        break;
                    case ZPlayMode.CooperativeHost:
                    case ZPlayMode.CooperativeClient:
                        _navigator.Navigate("View\\BF3CoopView.xaml");
                        break;
                    case ZPlayMode.Singleplayer:
                    case ZPlayMode.TestRange:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                // check can run game
                if (! _gameService.CanRun) return;

                if (playMode == ZPlayMode.Singleplayer)
                {
                    // create run params
                    var param = new SingleplayerJoinParams { Game = game };
                    // run game
                    _gameService.RunSingleplayer(param).Forget();
                }
                else
                {
                    // create run params
                    var param = new TestRangeJoinParams { Game = game };
                    // run game
                    _gameService.RunPlayground(param).Forget();
                }
            }
        }

        public ICommand ViewStatsCommand => new DelegateCommand(_ViewStatsCommandExec);

        private async void _ViewStatsCommandExec(object obj)
        {
            var connected = _state.GetState<bool>(Constants.ZCLIENT_CONNECTION);
            if (! connected)
            {
                _eventLogService.Log(EventLogLevel.Warning, HLM.StatsView, HLM.EventLauncherDisconnectedStats);
                return;
            }

            _busyService.Busy(HLM.BusyGettingStats);

            var game = (ZGame) Enum.Parse(typeof(ZGame), (string) obj);
            var stats = await _api.GetStatsAsync(game);
            var gameKey = game.ToString().ToLower();

            _state.SetState($"stats_{gameKey}", stats);

            if (stats.Rank == 0)
            {
                _eventLogService.Log(EventLogLevel.Warning, HLM.StatsView, HLM.EventNoobStats);
                _busyService.Free();
                return;
            }

            switch (game)
            {
                case ZGame.BF3:
                case ZGame.BF4:
                    _navigator.Navigate($"View\\StatsViews\\{game}StatsView.xaml"); break;
                case ZGame.BFH:
                case ZGame.None:
                    default: throw new ArgumentOutOfRangeException(nameof(game), game, @"Stats supports only in BF3 and BF4.");
            }

            _busyService.Free();
        }

        #endregion
    }
}
