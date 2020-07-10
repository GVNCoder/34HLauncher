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
                .UserControlViewModelLocator;
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
            var constructorArguments = _BuildConstructorParameters(new[] {"view", "process"}, _gameControl, process);

            _CreateGameAssistant(ref _gameHelper, typeof(DetectedGameImpl), constructorArguments);
        }

        public async Task Run(RunContext context)
        {
            if (! _canExecuteOperation) return;
            _canExecuteOperation = false;

            try
            {
                var gameSettings = _GetGameSettings(context.Target);
                var game = await _GetGameForRunAsync(context, gameSettings);

                var constructorParams = _BuildConstructorParameters(new[] {"game", "gameSettings", "context", "view"},
                    game, gameSettings, context, _gameControl);

                _CreateGameAssistant(ref _gameHelper, typeof(RunningGameImpl), constructorParams);

                var runResult = await game.RunAsync();
                if (runResult != ZRunResult.Success)
                {
                    throw new Exception("Cannot run game for unknown reason.");
                }
            }
            catch (Exception e)
            {
                _log.Error(LoggingHelper.GetMessage(e));
                _eventLogService.Log(EventLogLevel.Warning, "Game run error", e.Message);

                _canExecuteOperation = true;
                _DestroyGameAssistant(ref _gameHelper);
            }
        }

        private IGameHelper _BuildGameHelper(Type implType, IParameter[] parameters)
            => (IGameHelper) _kernel.Get(implType, parameters);

        private IParameter[] _BuildConstructorParameters(IEnumerable<string> paramNames, params object[] values)
            => paramNames.Select((t, i) => new ConstructorArgument(t, values[i])).Cast<IParameter>().ToArray();

        private void _CreateGameAssistant(ref IGameHelper gameHelperRef, Type implType, IParameter[] parameters)
        {
            gameHelperRef = _BuildGameHelper(implType, parameters);
            gameHelperRef.Close += _OnWorkCompeteHandler;
            gameHelperRef.BeginWork();
        }

        private void _DestroyGameAssistant(ref IGameHelper gameHelperRef)
        {
            gameHelperRef.Close -= _OnWorkCompeteHandler;
            gameHelperRef = null;
        }

        private void _OnWorkCompeteHandler(object sender, GameCloseEventArgs e)
        {
            _canExecuteOperation = true;
            _DestroyGameAssistant(ref _gameHelper);

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