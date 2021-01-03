using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using log4net;

using Launcher.Core.Services;
using Launcher.Core.Shared;

using Ninject;

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
        private readonly IZGameFactory _gameFactory;
        private readonly IKernel _kernel;
        private readonly ILog _log;
        
        private bool _canRunNewGame;
        private ZPlayMode _currentPlayMode;

        public GameService(
            IZApi api,
            App application,
            ISettingsService settingsService)
        {
            _kernel = Resolver.Kernel;

            _gameFactory = api.GameFactory;
            _settingsService = settingsService;
            _log = application.Logger;

            _canRunNewGame = true;
            _currentPlayMode = ZPlayMode.None;
        }

        #region IGameService

        public async Task RunMultiplayer(MultiplayerJoinParams parameters)
        {
            // check possibility to run a game
            var possibleToRun = _IsAlreadyHasRunGame();
            if (! possibleToRun || ! _canRunNewGame)
            {
                _OnCreationError("You already have an active game");
            }
            else
            {
                // set service state
                _canRunNewGame = false;
                _currentPlayMode = ZPlayMode.Multiplayer;

                // get game setting
                var settings = _GetGameSettings(parameters.Game);

                // create run params
                var runParams = new ZMultiParams
                {
                    Game = parameters.Game,
                    PreferredArchitecture = settings.PreferredArchitecture,
                    Role = parameters.PlayerRole,
                    ServerId = parameters.ServerModel.Id
                };

                try
                {
                    // create game to run
                    var gameProcess = await _gameFactory.CreateMultiAsync(runParams);

                    // create worker
                    var worker = _kernel.Get<MultiplayerGameWorker>();
                    worker.Complete += (sender, e) => _canRunNewGame = true;

                    // pass game created event
                    _OnCreated(worker, "Multiplayer", parameters.ServerModel.Name);

                    // run game
                    await worker.Begin(gameProcess, settings);
                }
                catch (Exception exc)
                {
                    // raise error event
                    _OnCreationError(exc.Message);

                    // reset service state
                    _canRunNewGame = true;
                    _currentPlayMode = ZPlayMode.None;
                }
            }
        }

        public async Task RunSingleplayer(SingleplayerJoinParams parameters)
        {
            // check possibility to run a game
            var possibleToRun = _IsAlreadyHasRunGame();
            if (! possibleToRun || ! _canRunNewGame)
            {
                _OnCreationError("You already have an active game");
            }
            else
            {
                // set service state
                _canRunNewGame = false;
                _currentPlayMode = ZPlayMode.Multiplayer;

                // get game setting
                var settings = _GetGameSettings(parameters.Game);

                // create run params
                var runParams = new ZSingleParams
                {
                    Game = parameters.Game,
                    PreferredArchitecture = settings.PreferredArchitecture
                };

                try
                {
                    // create game to run
                    var gameProcess = await _gameFactory.CreateSingleAsync(runParams);

                    // create worker
                    var worker = _kernel.Get<MultiplayerGameWorker>();
                    worker.Complete += (sender, e) => _canRunNewGame = true;

                    // pass game created event
                    _OnCreated(worker, "Singleplayer", "Campaign");

                    // run game
                    await worker.Begin(gameProcess, settings);
                }
                catch (Exception exc)
                {
                    // raise error event
                    _OnCreationError(exc.Message);

                    // reset service state
                    _canRunNewGame = true;
                    _currentPlayMode = ZPlayMode.None;
                }
            }
        }

        public async Task RunPlayground(TestRangeJoinParams parameters)
        {
            // check possibility to run a game
            var possibleToRun = _IsAlreadyHasRunGame();
            if (! possibleToRun || ! _canRunNewGame)
            {
                _OnCreationError("You already have an active game");
            }
            else
            {
                // set service state
                _canRunNewGame = false;
                _currentPlayMode = ZPlayMode.Multiplayer;

                // get game setting
                var settings = _GetGameSettings(parameters.Game);

                // create run params
                var runParams = new ZTestRangeParams
                {
                    Game = parameters.Game,
                    PreferredArchitecture = settings.PreferredArchitecture
                };

                try
                {
                    // create game to run
                    var gameProcess = await _gameFactory.CreateTestRangeAsync(runParams);

                    // create worker
                    var worker = _kernel.Get<TestRangeGameWorker>();
                    worker.Complete += (sender, e) => _canRunNewGame = true;

                    // pass game created event
                    _OnCreated(worker, "Singleplayer", "Playground");

                    // run game
                    await worker.Begin(gameProcess, settings);
                }
                catch (Exception exc)
                {
                    // raise error event
                    _OnCreationError(exc.Message);

                    // reset service state
                    _canRunNewGame = true;
                    _currentPlayMode = ZPlayMode.None;
                }
            }
        }

        public async Task RunCoop(CoopJoinParams parameters)
        {
            // check possibility to run a game
            var possibleToRun = _IsAlreadyHasRunGame();
            if (! possibleToRun || ! _canRunNewGame)
            {
                _OnCreationError("You already have an active game");
            }
            else
            {
                // set service state
                _canRunNewGame = false;
                _currentPlayMode = ZPlayMode.Multiplayer;

                // get game setting
                var settings = _GetGameSettings(parameters.Game);

                // create run params
                var runParams = new ZCoopParams
                {
                    PreferredArchitecture = settings.PreferredArchitecture,
                    Difficulty = parameters.CoopMission.Difficulty,
                    Level = parameters.CoopMission.Level,
                    Mode = parameters.Mode,
                    FriendId = parameters.FriendId
                };

                try
                {
                    // create game to run
                    var gameProcess = await _gameFactory.CreateCoOpAsync(runParams);

                    // create worker
                    var worker = _kernel.Get<CoopGameWorker>();
                    worker.Complete += (sender, e) => _canRunNewGame = true;

                    // pass game created event
                    _OnCreated(worker, "Coop", parameters.Mode == ZPlayMode.CooperativeClient ? "Client" : "Host");

                    // run game
                    await worker.Begin(gameProcess, settings);
                }
                catch (Exception exc)
                {
                    // raise error event
                    _OnCreationError(exc.Message);

                    // reset service state
                    _canRunNewGame = true;
                    _currentPlayMode = ZPlayMode.None;
                }
            }
        }

        public event EventHandler<GameCreatedEnventArgs> GameCreated;
        public event EventHandler<GameCreationErrorEventArgs> GameCreationError;

        public ZPlayMode CurrentPlayMode => _currentPlayMode;

        #endregion

        #region Private helpers

        private void _OnCreationError(string message) => GameCreationError?.Invoke(this, new GameCreationErrorEventArgs(message));

        private void _OnCreated(IGameWorker worker, string gameModeName, string placementName) =>
            GameCreated?.Invoke(this, new GameCreatedEnventArgs(worker, gameModeName, placementName));

        private bool _IsAlreadyHasRunGame()
        {
            var processes = Process.GetProcesses();
            var process = _GetAnyGameProcess(processes);

            return process == null;
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