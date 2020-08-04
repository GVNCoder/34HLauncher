﻿using System;
using System.Windows;
using System.Windows.Input;
using Launcher.Core.Bases;
using Launcher.Core.Data;
using Launcher.Core.Interaction;
using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Shared;
using Launcher.UserControls;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;

using HLM = Launcher.Localization.Loc.inCodeLocalizationMap.HomeViewLocalizationMap;
using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.ViewModel
{
    public class HomeViewModel : PageViewModelBase
    {
        private readonly IWindowContentNavigationService _navigationService;
        private readonly IEventLogService _eventLogService;
        private readonly IGameService _gameService;
        private readonly IContentPresenterService _presenterService;
        private readonly ISettingsService _settingsService;
        private readonly IZApi _api;
        private readonly IBusyService _busyService;

        public HomeViewModel(
            IWindowContentNavigationService navigationService,
            IEventLogService eventLogService,
            IGameService gameService,
            IContentPresenterService presenterService,
            ISettingsService settingsService,
            IZApi api,
            IBusyService busyService,
            IDiscord discord) : base(discord)
        {
            _navigationService = navigationService;
            _eventLogService = eventLogService;
            _gameService = gameService;
            _presenterService = presenterService;
            _settingsService = settingsService;
            _api = api;
            _busyService = busyService;
        }

        private bool _isOnline(object playMode)
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
            var targetGame = (ZGame) Enum.Parse(typeof(ZGame), (string) obj);
            var settings = _settingsService.GetGameSettings().Settings[(int) targetGame];
            var viewModel = new GameSettingsViewModel(settings, _settingsService, targetGame != ZGame.BF3);

            _presenterService.Show<GameSettingsControl>(viewModel);
        }

        public ICommand JoinGameCommand => new DelegateCommand(_JoinGameCommandExec);

        private static T _parseEnum<T>(string value) => (T) Enum.Parse(typeof(T), value);

        private void _JoinGameCommandExec(object obj)
        {
            var parameters = (obj as string)
                .Split('.');
            var game = _parseEnum<ZGame>(parameters[0]);
            var playMode = _parseEnum<ZPlayMode>(parameters[1]);

            var connected = (bool)State.Storage["connection"];
            if (!connected)
            {
                if (_isOnline(playMode))
                {
                    _eventLogService.Log(EventLogLevel.Warning, SLM.GameRun, HLM.EventLauncherDisconnectedServers);
                }
                else
                {
                    _eventLogService.Log(EventLogLevel.Warning, SLM.GameRun, HLM.EventLauncherDisconnected);
                }

                return;
            }

            if (_isOnline(playMode))
            {
                switch (playMode)
                {
                    case ZPlayMode.Multiplayer: _navigationService.NavigateTo($"View\\{game}MultiplayerView.xaml");
                        break;
                    case ZPlayMode.CooperativeHost:
                    case ZPlayMode.CooperativeClient:
                        _navigationService.NavigateTo("View\\BF3CoopView.xaml");
                        break;
                }
            }
            else
            {
                //var context = new RunContext
                //{
                //    Target = (ZGame) game,
                //    Mode = (ZPlayMode) playMode
                //};
                //_gameService.Run(context);

                // check can run game
                if (! _gameService.CanRun) return;

                if (playMode == ZPlayMode.Singleplayer)
                {
                    // create run params
                    var param = new SingleplayerJoinParams { Game = game };
                    // run game
                    _gameService.RunSingleplayer(param);
                }
                else
                {
                    // create run params
                    var param = new TestRangeJoinParams { Game = game };
                    // run game
                    _gameService.RunPlayground(param);
                }
            }
        }

        public ICommand ViewStatsCommand => new DelegateCommand(_ViewStatsCommandExec);

        private async void _ViewStatsCommandExec(object obj)
        {
            var connected = (bool)State.Storage["connection"];
            if (!connected)
            {
                _eventLogService.Log(EventLogLevel.Warning, HLM.StatsView, HLM.EventLauncherDisconnectedStats);
                return;
            }

            _busyService.Busy(HLM.BusyGettingStats);

            var game = (ZGame) Enum.Parse(typeof(ZGame), (string) obj);
            var stats = await _api.GetStatsAsync(game);

            State.Storage[$"stats_{game}"] = stats;

            if (stats.Rank == 0)
            {
                _eventLogService.Log(EventLogLevel.Warning, HLM.StatsView, HLM.EventNoobStats);
                _busyService.Free();
                return;
            }

            switch (game)
            {
                case ZGame.BF3:
                    _navigationService.NavigateTo("View\\StatsViews\\BF3StatsView.xaml"); break;
                case ZGame.BF4:
                    _navigationService.NavigateTo("View\\StatsViews\\BF4StatsView.xaml"); break;
                case ZGame.BFH:
                    //_navigationService.NavigateTo("View\\StatsViews\\BFHStatsView.xaml"); break;
                case ZGame.None:
                    default: throw new ArgumentOutOfRangeException(nameof(game), game, @"Stats supports only in BF3 and BF4.");
            }

            _busyService.Free();
        }

        #endregion
    }
}
