using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using log4net;
using Launcher.Core.Services;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Ninject;
using Ninject.Parameters;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.Core.Data
{
    public class GameService : IGameService
    {
        private const int __x64ProcessNameLength = 3;
        private const int __x86ProcessNameLength = 7;

        private readonly IEventLogService _eventLogService;
        private readonly ISettingsService _settingsService;
        private readonly IGameControl _gameControl;
        private readonly IZGameFactory _runService;
        private readonly IKernel _kernel;
        private readonly ILog _log;
        
        private IGameHelper _gameHelper;
        private bool _canExecuteOperation;

        public GameService(
            IZApi api,
            App application,
            IEventLogService eventLogService,
            ISettingsService settingsService)
        {
            _canExecuteOperation = true;

            var viewModelLocator = application
                .DependencyResolver
                .Locators
                .ViewModelLocator;
            _gameControl = viewModelLocator.GameControlViewModel;

            _kernel = application.DependencyResolver.Resolver;

            _runService = api.GameFactory;
            _eventLogService = eventLogService;
            _settingsService = settingsService;
            _log = application.Logger;
        }

        public void TryDetect()
        {
            var processes = Process.GetProcesses();
            var process = _GetAnyGameProcess(processes);

            if (process == null) return;

            _canExecuteOperation = false;

            var viewArgument = new ConstructorArgument("view", _gameControl);
            var processArgument = new ConstructorArgument("process", process);

            _gameHelper = _kernel.Get<DetectedGameImpl>(viewArgument, processArgument);
            _gameHelper.Close += _OnWorkCompeteHandler;
            _gameHelper.BeginWork();
        }

        public async Task Run(RunContext context)
        {
            if (! _canExecuteOperation) return;
            _canExecuteOperation = false;

            try
            {
                var gameSettings = _GetGameSettings(context.Target);
                var game = await _GetGameForRunAsync(context, gameSettings);

                var gameArgument = new ConstructorArgument("game", game);
                var gameSettingsArgument = new ConstructorArgument("gameSettings", gameSettings);
                var contextArgument = new ConstructorArgument("context", context);
                var viewArgument = new ConstructorArgument("view", _gameControl);

                _gameHelper = _kernel.Get<RunningGameImpl>(gameArgument, gameSettingsArgument, contextArgument, viewArgument);
                _gameHelper.Close += _OnWorkCompeteHandler;
                _gameHelper.BeginWork();

                var runResult = await game.RunAsync();
                if (runResult != ZRunResult.Success)
                {
                    _eventLogService.Log(EventLogLevel.Warning, "Game run error", "Cannot run game for unknown reason.");
                    _canExecuteOperation = true;
                }
            }
            catch (Exception e)
            {
                _log.Error(LoggingHelper.GetMessage(e));
                _eventLogService.Log(EventLogLevel.Warning, "Game run error", e.Message);
                _canExecuteOperation = true;
            }
        }

        private void _OnWorkCompeteHandler(object sender, GameCloseEventArgs e)
        {
            _canExecuteOperation = true;
            _gameHelper.Close -= _OnWorkCompeteHandler;

            if (!string.IsNullOrEmpty(e.PipeContent))
            {
                _eventLogService.Log(EventLogLevel.Message, SLM.GameRun, e.PipeContent);
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
    }
}