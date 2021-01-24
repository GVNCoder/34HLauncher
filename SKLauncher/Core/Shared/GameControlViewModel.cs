using System;
using System.Windows;
using System.Windows.Input;

using Launcher.Core.Data;
using Launcher.Core.Interaction;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;

namespace Launcher.Core.Shared
{
    public class GameControlViewModel : BaseControlViewModel
    {
        private readonly IEventService _eventService;

        private IGameWorker _worker;

        public GameControlViewModel(
            IGameService gameService
            , IEventService eventService)
        {
            _eventService = eventService;

            // track service events
            gameService.GameCreated += _GameCreatedHandler;
        }

        #region Dependency properties

        public string GameModeName
        {
            get => (string) Dispatcher.Invoke(() => GetValue(GameModeNameProperty));
            set => Dispatcher.Invoke(() => SetValue(GameModeNameProperty, value));
        }
        public static readonly DependencyProperty GameModeNameProperty =
            DependencyProperty.Register("GameModeName", typeof(string), typeof(GameControlViewModel), new PropertyMetadata(string.Empty));

        public string PlacementName
        {
            get => (string)Dispatcher.Invoke(() => GetValue(PlacementNameProperty));
            set => Dispatcher.Invoke(() => SetValue(PlacementNameProperty, value));
        }
        public static readonly DependencyProperty PlacementNameProperty =
            DependencyProperty.Register("PlacementName", typeof(string), typeof(GameControlViewModel), new PropertyMetadata(string.Empty));

        public bool IsGameControlVisible
        {
            get => (bool)Dispatcher.Invoke(() => GetValue(IsGameControlVisibleProperty));
            set => Dispatcher.Invoke(() => SetValue(IsGameControlVisibleProperty, value));
        }
        public static readonly DependencyProperty IsGameControlVisibleProperty =
            DependencyProperty.Register("IsGameControlVisible", typeof(bool), typeof(GameControlViewModel), new PropertyMetadata(false));

        public bool IsLoadingIndicatorVisible
        {
            get => (bool) Dispatcher.Invoke(() => GetValue(IsLoadingIndicatorVisibleProperty));
            set => Dispatcher.Invoke(() => SetValue(IsLoadingIndicatorVisibleProperty, value));
        }
        public static readonly DependencyProperty IsLoadingIndicatorVisibleProperty =
            DependencyProperty.Register("IsLoadingIndicatorVisible", typeof(bool), typeof(GameControlViewModel), new PropertyMetadata(true));

        public bool IsCloseButtonVisible
        {
            get => (bool) Dispatcher.Invoke(() => GetValue(IsCloseButtonVisibleProperty));
            set => Dispatcher.Invoke(() => SetValue(IsCloseButtonVisibleProperty, value));
        }
        public static readonly DependencyProperty IsCloseButtonVisibleProperty =
            DependencyProperty.Register("IsCloseButtonVisible", typeof(bool), typeof(GameControlViewModel), new PropertyMetadata(false));

        #endregion

        #region Commands

        public ICommand CloseCommand => new DelegateCommand(obj =>
        {
            // hide close button
            IsCloseButtonVisible = false;

            // send a request to cancel game
            _worker.Stop();
        });

        public override ICommand LoadedCommand => throw new NotImplementedException();

        public override ICommand UnloadedCommand => throw new NotImplementedException();

        #endregion

        #region Private helpers

        private void _GameCreatedHandler(object sender, GameCreatedEnventArgs e)
        {
            _worker = e.Worker;

            // set visuals state
            IsGameControlVisible = true;
            GameModeName = e.GamePlayModeName;
            PlacementName = e.PlacementName;

            // track game worker events
            _worker.Error += _WorkerErrorHandler;
            _worker.Complete += _WorkerCompleteHandler;
            _worker.GamePipe += _WorkerGamePipeHandler;
            _worker.CanCloseGame += _WorkerCanCloseHandler;
            _worker.GameLoadingCompleted += _WorkerGameLoadingCompleted;
        }

        private void _WorkerGameLoadingCompleted(object sender, EventArgs e)
        {
            // change internal state
            IsLoadingIndicatorVisible = false;
        }

        private void _WorkerCanCloseHandler(object sender, EventArgs e)
        {
            // change internal state
            IsCloseButtonVisible = true;
        }

        private void _WorkerGamePipeHandler(object sender, GameWorkerPipeLogEventArgs e)
        {
            _eventService.InfoEvent("Game run", e.PipeLog);
        }

        private void _WorkerCompleteHandler(object sender, EventArgs e)
        {
            // change internal state
            IsGameControlVisible = false;
            IsCloseButtonVisible = false;
            IsLoadingIndicatorVisible = true;

            _worker.Error -= _WorkerErrorHandler;
            _worker.Complete -= _WorkerCompleteHandler;
            _worker.GamePipe -= _WorkerGamePipeHandler;
            _worker.CanCloseGame -= _WorkerCanCloseHandler;
            _worker.GameLoadingCompleted -= _WorkerGameLoadingCompleted;
        }

        private void _WorkerErrorHandler(object sender, GameWorkerErrorEventArgs e)
        {
            _eventService.WarnEvent("Game run", e.Message);
        }

        #endregion
    }
}