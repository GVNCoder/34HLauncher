using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using log4net;

using Launcher.Core.Services;
using Launcher.Core.Shared;

using Ninject;
using Ninject.Parameters;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

namespace Launcher.Core.Data
{
    public class GameService : IGameService
    {
        private const int __x64ProcessNameLength = 3;
        private const int __x86ProcessNameLength = 7;

        private readonly ISettingsService _settingsService;
        private readonly IGameControl _gameControl;
        private readonly IZGameFactory _gameFactory;
        private readonly IKernel _kernel;
        private readonly ILog _log;
        
        private BaseGameWorker _gameWorker;

        public GameService(
            IZApi api,
            App application,
            ISettingsService settingsService)
        {
            var viewModelLocator = application
                .DependencyResolver
                .Locators
                .UserControlViewModelLocator;
            _gameControl = viewModelLocator.GameControlViewModel;

            _kernel = application.DependencyResolver.Resolver;

            _gameFactory = api.GameFactory;
            _settingsService = settingsService;
            _log = application.Logger;

            CanRun = true;
        }

        #region IGameService

        public async Task RunMultiplayer(MultiplayerJoinParams param)
        {
            // disallow game run
            CanRun = false;
            // set current play mode
            CurrentPlayMode = ZPlayMode.Multiplayer;

            // get game setting
            var settings = _GetGameSettings(param.Game);
            // create run params
            var runParams = new ZMultiParams
            {
                Game = param.Game,
                PreferredArchitecture = settings.PreferredArchitecture,
                Role = param.PlayerRole,
                ServerId = param.ServerModel.Id
            };
            try
            {
                // create game for run
                var gameForRun = await _gameFactory.CreateMultiAsync(runParams);
                var error = await _TryRunAndStartWorker(typeof(MultiplayerGameWorker), settings, param, gameForRun);

                // throw run error if exists
                if (error != null) throw error;
            }
            catch (Exception exc)
            {
                // raise error event
                _OnGameRunError(exc);
                // allow to run
                CanRun = true;
                // reset current play mode
                CurrentPlayMode = null;
            }
        }

        public async Task RunSingleplayer(SingleplayerJoinParams param)
        {
            // disallow game run
            CanRun = false;
            // set current play mode
            CurrentPlayMode = ZPlayMode.Singleplayer;

            // get game setting
            var settings = _GetGameSettings(param.Game);
            // create run params
            var runParams = new ZSingleParams
            {
                Game = param.Game,
                PreferredArchitecture = settings.PreferredArchitecture
            };
            try
            {
                // create game for run
                var gameForRun = await _gameFactory.CreateSingleAsync(runParams);
                var error = await _TryRunAndStartWorker(typeof(SingleplayerGameWorker), settings, param, gameForRun);

                // throw run error if exists
                if (error != null) throw error;
            }
            catch (Exception exc)
            {
                // raise error event
                _OnGameRunError(exc);
                // allow to run
                CanRun = true;
                // reset current play mode
                CurrentPlayMode = null;
            }
        }

        public async Task RunPlayground(TestRangeJoinParams param)
        {
            // disallow game run
            CanRun = false;
            // set current play mode
            CurrentPlayMode = ZPlayMode.TestRange;

            // get game setting
            var settings = _GetGameSettings(param.Game);
            // create run params
            var runParams = new ZTestRangeParams
            {
                Game = param.Game,
                PreferredArchitecture = settings.PreferredArchitecture
            };
            try
            {
                // create game for run
                var gameForRun = await _gameFactory.CreateTestRangeAsync(runParams);
                var error = await _TryRunAndStartWorker(typeof(TestRangeGameWorker), settings, param, gameForRun);

                // throw run error if exists
                if (error != null) throw error;
            }
            catch (Exception exc)
            {
                // raise error event
                _OnGameRunError(exc);
                // allow to run
                CanRun = true;
                // reset current play mode
                CurrentPlayMode = null;
            }
        }

        public async Task RunCoop(CoopJoinParams param)
        {
            // disallow game run
            CanRun = false;
            // set current play mode
            CurrentPlayMode = param.Mode;

            // get game setting
            var settings = _GetGameSettings(param.Game);
            // create run params
            ZCoopParams runParams = null;
            if (param.Mode == ZPlayMode.CooperativeClient)
            {
                runParams = new ZCoopParams
                {
                    PreferredArchitecture = settings.PreferredArchitecture,
                    Mode = param.Mode,
                    FriendId = param.FriendId
                };
            }
            else
            {
                runParams = new ZCoopParams
                {
                    PreferredArchitecture = settings.PreferredArchitecture,
                    Difficulty = param.CoopMission.Difficulty,
                    Level = param.CoopMission.Level,
                    Mode = param.Mode,
                    FriendId = param.FriendId
                };
            }

            try
            {
                // create game for run
                var gameForRun = await _gameFactory.CreateCoOpAsync(runParams);
                var error = await _TryRunAndStartWorker(typeof(CoopGameWorker), settings, param, gameForRun);

                // throw run error if exists
                if (error != null) throw error;
            }
            catch (Exception exc)
            {
                // raise error event
                _OnGameRunError(exc);
                // allow to run
                CanRun = true;
                // reset current play mode
                CurrentPlayMode = null;
            }
        }

        public void TryDetect()
        {
            var processes = Process.GetProcesses();
            var process = _GetAnyGameProcess(processes);

            if (process == null) return;

            CanRun = false;
            var constructorArguments = _BuildConstructorParameters(new[] { "view", "process" }, _gameControl, process);

            _CreateGameWorker(ref _gameWorker, typeof(DetectedGameWorker), constructorArguments);
        }

        public event EventHandler<GameCloseEventArgs> GameClose;
        public event EventHandler<GameRunErrorEventArgs> GameRunError;

        public bool CanRun { get; private set; }
        public BaseGameWorker CurrentGame => _gameWorker;
        public ZPlayMode? CurrentPlayMode { get; private set; }

        #endregion

        #region Private helpers

        private void _OnGameClose(GameCloseEventArgs e) => GameClose?.Invoke(this, e);
        private void _OnGameRunError(Exception error) => GameRunError?.Invoke(this, new GameRunErrorEventArgs(error));

        private async Task<Exception> _TryRunAndStartWorker(Type implType, GameSetting gameSettings, BaseJoinParams param, IZRunGame game)
        {
            Exception exception = null;
            try
            {
                var constructorParams = _BuildConstructorParameters(new[] { "game", "gameSettings", "param", "view" },
                    game, gameSettings, param, _gameControl);

                _CreateGameWorker(ref _gameWorker, implType, constructorParams);

                var runResult = await game.RunAsync();
                if (runResult != ZRunResult.Success)
                {
                    throw new Exception("Cannot run game for unknown reason.");
                }
            }
            catch (Exception exc)
            {
                _DestroyGameAssistant(ref _gameWorker);
                exception = exc;
            }

            return exception;
        }

        private BaseGameWorker _BuildGameWorker(Type implType, IParameter[] parameters)
            => (BaseGameWorker)_kernel.Get(implType, parameters);

        private static IParameter[] _BuildConstructorParameters(IEnumerable<string> paramNames, params object[] values)
            => paramNames.Select((t, i) => new ConstructorArgument(t, values[i])).Cast<IParameter>().ToArray();

        // ReSharper disable once RedundantAssignment
        private void _CreateGameWorker(ref BaseGameWorker gameWorkerRef, Type implType, IParameter[] parameters)
        {
            gameWorkerRef = _BuildGameWorker(implType, parameters);
            gameWorkerRef.Done += _OnWorkCompeteHandler;
            gameWorkerRef.BeginWork();
        }

        private void _DestroyGameAssistant(ref BaseGameWorker gameWorkerRef)
        {
            gameWorkerRef.Done -= _OnWorkCompeteHandler;
            gameWorkerRef = null;
        }

        private void _OnWorkCompeteHandler(object sender, GameCloseEventArgs e)
        {
            CanRun = true;
            // reset current play mode
            CurrentPlayMode = null;

            _DestroyGameAssistant(ref _gameWorker);
            if (!string.IsNullOrEmpty(e.PipeLog))
            {
                _OnGameClose(e);
            }
        }

        private static Process _GetAnyGameProcess(IEnumerable<Process> processes)
        {
            bool __IsAGameProcess(Process process) => process.ProcessName.StartsWith("bf", StringComparison.OrdinalIgnoreCase);
            bool __IsX64GameProcess(Process process) => process.ProcessName.Length == __x64ProcessNameLength;
            bool __IsX86GameProcess(Process process) => process.ProcessName.Length == __x86ProcessNameLength &&
                                                        process.ProcessName.EndsWith("_x86", StringComparison.OrdinalIgnoreCase);
            return processes
                // ReSharper disable once ConvertClosureToMethodGroup
                .Where(p => __IsAGameProcess(p))
                .FirstOrDefault(p => __IsX64GameProcess(p) || __IsX86GameProcess(p));
        }

        private GameSetting _GetGameSettings(ZGame target)
            => _settingsService
                .GetGameSettings()
                .Settings[(int)target];

        #endregion
    }
}