using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using log4net;
using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Ninject;
using Ninject.Parameters;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;

using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.Core.Data
{
    public class GameService : IGameService
    {
        //private const string __GameLoading = "GameLoading";
        //private const string __WaitForPeerClient = "WaitForPeerClient";
        //private const string __GameWaiting = "GameWaiting";

        private const int __x64ProcessNameLength = 3;
        private const int __x86ProcessNameLength = 7;

        private readonly IEventLogService _eventLogService;
        private readonly ISettingsService _settingsService;
        private readonly IDiscord _discord;
        private readonly IGameControl _gameControl;
        private readonly IZGameFactory _runService;
        private readonly ILog _log;

        private readonly IKernel _kernel;

        private ZProcessTracker _processTracker;
        private GameSetting _gameSettings;
        private string _pipeContent;
        private RunContext _context;
        private IZRunGame _game;

        private IGameHelper _gameHelper;
        private bool _canExecuteOperation;

        public GameService(
            IZApi api,
            App application,
            IEventLogService eventLogService,
            ISettingsService settingsService,
            IDiscord discord)
        {
            _canExecuteOperation = true;

            var viewModelLocator = application
                .DependencyResolver
                .Locators
                .ViewModelLocator;
            _gameControl = viewModelLocator.GameControlViewModel;
            //_gameControl.CloseClick += _closeGameRequestHandler;

            _kernel = application.DependencyResolver.Resolver;

            _runService = api.GameFactory;
            _eventLogService = eventLogService;
            _settingsService = settingsService;
            _log = application.Logger;
            _discord = discord;
        }

        public void TryDetect()
        {
            //void __ProcessDetected(object sender, Process e)
            //{
            //    var tracker = (IZProcessTracker) sender;
            //    tracker.ProcessDetected -= __ProcessDetected;

            //    _gameControl.SetCanClose(true);

            //    _discord.UpdateUnknown();
            //}

            //void __ProcessLost(object sender, EventArgs e)
            //{
            //    var tracker = (IZProcessTracker) sender;
            //    tracker.ProcessLost -= __ProcessLost;

            //    _gameControl.CloseClick -= __OnCloseClickHandler;
            //    _gameControl.Hide();

            //    _discord.DisablePlay();
            //}

            //void __OnCloseClickHandler(object sender, EventArgs e)
            //{
            //    _processTracker.Process?.Kill();
            //}

            var processes = Process.GetProcesses();
            //var process = _GetAnyGameProcess(processes)?.ProcessName;
            var process = _GetAnyGameProcess(processes);

            if (process == null) return;

            _canExecuteOperation = false;

            var viewArgument = new ConstructorArgument("view", _gameControl);
            var processArgument = new ConstructorArgument("process", process);

            _gameHelper = _kernel.Get<DetectedGameImpl>(viewArgument, processArgument);
            _gameHelper.Close += _OnWorkCompeteHandler;
            _gameHelper.BeginWork();

            //_SetUnknownVisual();
            //_gameControl.CloseClick += __OnCloseClickHandler;
            //_gameControl.Show();

            //_processTracker = new ZProcessTracker(process, TimeSpan.FromSeconds(3), false, ps => ps.FirstOrDefault());
            //_processTracker.ProcessDetected += __ProcessDetected;
            //_processTracker.ProcessLost += __ProcessLost;
            //_processTracker.StartTrack();
        }

        private void _OnWorkCompeteHandler(object sender, GameCloseEventArgs e)
        {
            _canExecuteOperation = true;
            _gameHelper.Close -= _OnWorkCompeteHandler;

            if (! string.IsNullOrEmpty(e.PipeContent))
            {
                _eventLogService.Log(EventLogLevel.Message, SLM.GameRun, e.PipeContent);
            }
        }

        public async Task Run(RunContext context)
        {
            if (! _canExecuteOperation) return;

            //_context = context;

            //_SetGameVisual(context);
            //_gameControl.Show();

            _canExecuteOperation = false;

            try
            {
                var gameSettings = _GetGameSettings(context.Target);
                var game = await _GetGameForRunAsync(context, gameSettings);

                var runResult = await game.RunAsync();
                if (runResult == ZRunResult.Success)
                {
                    var gameArgument = new ConstructorArgument("game", game);
                    var gameSettingsArgument = new ConstructorArgument("gameSettings", gameSettings);
                    var contextArgument = new ConstructorArgument("context", context);
                    var viewArgument = new ConstructorArgument("view", _gameControl);

                    _gameHelper = _kernel.Get<RunningGameImpl>(gameArgument, gameSettingsArgument, contextArgument, viewArgument);
                    _gameHelper.Close += _OnWorkCompeteHandler;
                    _gameHelper.BeginWork();
                }
                else
                {
                    _eventLogService.Log(EventLogLevel.Warning, "Game run error", "Cannot run game for unknown reason.");
                    _canExecuteOperation = true;
                }

                //_gameSettings = _GetGameSettings(context.Target);
                //_game = await _GetGameForRunAsync(context, _gameSettings);

                //var runResult = await _game.RunAsync();
                //if (runResult == ZRunResult.Success)
                //{
                //    _game.Pipe += _pipeHandler;
                //}
                //else
                //{
                //    _eventLogService.Log(EventLogLevel.Warning, "Game run error", "Cannot run game for unknown reason.");
                //    _Reset();
                //}
            }
            catch (Exception e)
            {
                _log.Error(LoggingHelper.GetMessage(e));
                _eventLogService.Log(EventLogLevel.Warning, "Game run error", e.Message);
                _canExecuteOperation = true;

                //_Reset();
            }
        }

        private Process _GetAnyGameProcess(IEnumerable<Process> processes)
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
                .Settings[(int) target];

        //public event EventHandler<GameEventArgs> GameEvent;

        //private void _OnGameEvent(GameEventType eventType, string pipeContent, GameSetting settings, IZRunGame game)
        //    => GameEvent?.Invoke(this, new GameEventArgs(eventType, pipeContent, settings, game));

        //private void _Reset()
        //{
        //    _context = null;
        //    _gameControl.Hide();
        //}

        private async Task<IZRunGame> _GetGameForRunAsync(RunContext context, GameSetting settings)
        {
            switch (context.Mode)
            {
                case ZPlayMode.Singleplayer:
                    return await _runService.CreateSingleAsync(new ZSingleParams { Game = context.Target, PreferredArchitecture = settings.PreferredArchitecture });
                case ZPlayMode.TestRange:
                    return await _runService.CreateTestRangeAsync(new ZTestRangeParams { Game = context.Target, PreferredArchitecture = settings.PreferredArchitecture });
                case ZPlayMode.Multiplayer:
                    return await _runService.CreateMultiAsync(new ZMultiParams
                        { Game = context.Target, Role = context.Role, ServerId = context.Server.Id, PreferredArchitecture = settings.PreferredArchitecture });
                case ZPlayMode.CooperativeClient:
                case ZPlayMode.CooperativeHost:
                    return await _runService.CreateCoOpAsync(new ZCoopParams
                        { Difficulty = context.Difficulty, Level = context.Level, Mode = context.Mode, FriendId = context.FriendId, PreferredArchitecture = settings.PreferredArchitecture });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //private void _closeGameRequestHandler(object sender, EventArgs e)
        //{
        //    _game?.TryClose();
        //}

        //private void _pipeHandler(object sender, ZGamePipeArgs e)
        //{
        //    _AppendPipeContent(e.FullMessage);
        //    _gameControl.SetCanClose(true);

        //    if (e.SecondPart.Contains(__GameLoading) || e.SecondPart.Contains(__WaitForPeerClient))
        //    {
        //        _OnGameEvent(GameEventType.LevelLoading, null, _gameSettings, _game);
        //    }

        //    if (e.FirstPart == __GameWaiting)
        //    {
        //        _OnGameEvent(GameEventType.Waiting, null, _gameSettings, _game);
        //        _SetPlayingVisual();
        //    }

        //    if (_game.IsRun) return;

        //    _OnGameEvent(GameEventType.Close, _pipeContent, _gameSettings, _game);

        //    _game.Pipe -= _pipeHandler;
        //    _pipeContent = string.Empty;
        //    _Reset();

        //    _discord.DisablePlay();
        //}

        //private void _AppendPipeContent(string content)
        //    => _pipeContent += string.IsNullOrEmpty(_pipeContent) ? content : $"\n{content}";

        #region View

        //private void _SetUnknownVisual()
        //{
        //    _gameControl.SetCanClose(false);
        //    _gameControl.SetState(false);
        //    _gameControl.SetText("Playing");
        //    _gameControl.SetToolTipText("Unknown mode");
        //}

        //private void _SetGameVisual(RunContext context)
        //{
        //    _SetContextVisual(context);

        //    _gameControl.SetState(true);
        //    _gameControl.SetCanClose(false);
        //}

        //private void _SetContextVisual(RunContext context)
        //{
        //    switch (context.Mode)
        //    {
        //        case ZPlayMode.Singleplayer:
        //            _gameControl.SetText("Resume campaign");
        //            _gameControl.SetToolTipText("Campaign");

        //            _discord.UpdateSingle(context.Target, ZPlayMode.Singleplayer);

        //            break;
        //        case ZPlayMode.Multiplayer:
        //            var server = context.Server;

        //            _gameControl.SetText("Joining server");
        //            _gameControl.SetToolTipText($"{server.Name} | {server.CurrentMap.Name} | {server.CurrentMap.GameModeName}");

        //            _discord.UpdateServer(server);

        //            break;
        //        case ZPlayMode.CooperativeHost:
        //            _gameControl.SetText("Host room");
        //            _gameControl.SetToolTipText("Host CoOp room");

        //            _discord.UpdateCoop(context.Mode, context.Mission);

        //            break;
        //        case ZPlayMode.CooperativeClient:
        //            _gameControl.SetText("Joining friend");
        //            _gameControl.SetToolTipText("Playing CoOp with friend");

        //            _discord.UpdateCoop(context.Mode, context.Mission);

        //            break;
        //        case ZPlayMode.TestRange:
        //            _gameControl.SetText("Playground");
        //            _gameControl.SetToolTipText("Test range");

        //            _discord.UpdateSingle(context.Target, ZPlayMode.TestRange);

        //            break;
        //        default:
        //            _log.Warn("PlayMode context has wrong value.", new ArgumentOutOfRangeException(nameof(context.Mode), context.Mode, "Out of enum range"));
        //            break;
        //    }
        //}

        //private void _SetPlayingVisual()
        //{
        //    _gameControl.SetText("Playing");
        //    _gameControl.SetState(false);
        //}

        #endregion
    }
}