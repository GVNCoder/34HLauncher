using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Shared;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;

namespace Launcher.Core.Data
{
    public class SingleplayerGameWorker : IGameWorker
    {
        private readonly ISettingsService _settingsService;
        private readonly StringBuilder _pipeLogger;
        private readonly IDiscord _discord;
        private readonly IZApi _zloApi;
        private readonly ILog _logger;

        private IZGameProcess _gameProcess;
        private GameSetting _gameSettings;

        public SingleplayerGameWorker(
            ISettingsService settingsService
            , IDiscord discord
            , IZApi zloApi
            , ILog logger)
        {
            _settingsService = settingsService;
            _discord = discord;
            _zloApi = zloApi;
            _logger = logger;

            _pipeLogger = new StringBuilder();
        }

        #region IGameWorker

        public async Task Begin(IZGameProcess gameProcess, GameSetting gameSettings, BaseJoinParams parameters)
        {
            _gameProcess = gameProcess;
            _gameSettings = gameSettings;

            // track game pipe
            _gameProcess.StateChanged += _gamePipeHandler;

            // run game and go to work ;)
            var runResult = await _gameProcess.RunAsync();
            if (runResult != ZRunResult.Success)
            {
                // log some info
                var message = $"For {runResult} reason can't run game";

                _logger.Error(message);
                _OnError(message);
                _OnWorkComplete();
            }
            else
            {
                // extract singleplayer parameters
                var singleplayerParameters = (SingleplayerJoinParams)parameters;

                // enable server discord presence
                _discord.UpdateSingle(singleplayerParameters.Game, ZPlayMode.Singleplayer);
            }
        }

        public void Stop()
        {
            var closeResult = _gameProcess.TryClose();
            if (closeResult) return;

            // log some info
            const string message = "For some reason can't close game process";

            _logger.Warn(message);
            _OnError(message);
        }

        public event EventHandler<GameWorkerErrorEventArgs> Error;
        public event EventHandler<GameWorkerPipeLogEventArgs> GamePipe;
        public event EventHandler CanCloseGame;
        public event EventHandler GameLoadingCompleted;
        public event EventHandler Complete;

        #endregion

        #region Private helpers

        private void _OnWorkComplete() => Complete?.Invoke(this, EventArgs.Empty);

        private void _OnError(string message) => Error?.Invoke(this, new GameWorkerErrorEventArgs(message));

        private void _OnCanCloseGame() => CanCloseGame?.Invoke(this, EventArgs.Empty);

        private void _OnGameLoadingCompleted() => GameLoadingCompleted?.Invoke(this, EventArgs.Empty);

        private void _OnGamePipeLog(string gamePipe) => GamePipe?.Invoke(this, new GameWorkerPipeLogEventArgs(gamePipe));

        private void _gamePipeHandler(object sender, ZGamePipeArgs e)
        {
            // save part of pipe log
            _pipeLogger.AppendLine(e.RawFullMessage);

            // handle pipe event
            switch (e.Event)
            {
                case ZGameEvent.Unknown:

                    // ignore but log, it's important to understand, what happen here
                    _logger.Warn($"Received unknown event with next value {e.RawFullMessage}");

                    break;
                case ZGameEvent.StateChanged:

                    _handleStates(e.States);

                    break;
                case ZGameEvent.Alert:
                    break;
                case ZGameEvent.GameWaiting:

                    // we're already loaded
                    _OnGameLoadingCompleted();

                    // inject selected dll`s
                    if (!_settingsService.GlobalBlock)
                    {
                        _zloApi.InjectDll(_gameSettings.Game, _gameSettings.Dlls);
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void _handleStates(ZGameState[] states)
        {
            // we're have a live game process
            if (states.Contains(ZGameState.State_GameRun))
            {
                _OnCanCloseGame();
            }
            // unfold game window
            else if (states.Contains(ZGameState.State_GameLoading))
            {
                var launcherSettings = _settingsService.GetLauncherSettings();
                // ReSharper disable once InvertIf
                if (!_settingsService.GlobalBlock && launcherSettings.UnfoldGameWindow)
                {
                    var unfoldResult = _gameProcess.TryUnfoldGameWindow();
                    // ReSharper disable once InvertIf
                    if (!unfoldResult)
                    {
                        const string message = "For some reason can't unfold game window";

                        _OnError(message);
                        _logger.Warn(message);
                    }
                }
            }
            else if (states.Contains(ZGameState.State_GameClose))
            {
                _OnGamePipeLog(_pipeLogger.ToString());
                _OnWorkComplete();

                // disable discord server presence
                _discord.ToggleGame();
            }
        }

        #endregion
    }
}