using System;

using Launcher.Core.RPC;
using Launcher.Core.Shared;
using Launcher.Core.Services;

using Zlo4NET.Api;
using Zlo4NET.Api.Service;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Data
{
    public abstract class BaseRunningGameWorker : BaseGameWorker
    {
        private const string __GameLoading = "GameLoading";
        private const string __WaitForPeerClient = "WaitForPeerClient";
        private const string __GameWaiting = "GameWaiting";
        private const string __GameRunning = "GameRunning";

        private readonly ISettingsService _settingsService;
        private readonly GameSetting _gameSettings;
        private readonly IZRunGame _game;
        private readonly IZApi _api;

        private string _pipeContent;

        protected BaseRunningGameWorker(
            IZRunGame game,
            GameSetting gameSettings,
            BaseJoinParams param,
            IGameControl view,
            IZApi api,
            ISettingsService settingsService,
            IDiscord discord) : base(discord, view)
        {
            _settingsService = settingsService;
            _gameSettings = gameSettings;
            _game = game;
            _api = api;

            _view.CloseClick += _OnCloseRequestedHandler;

            _game.Pipe += _pipeHandler;
            _SetGameVisual(param);
        }

        public override void BeginWork()
        {
            _view.Show();
        }

        #region Private methods

        private void _OnCloseRequestedHandler(object sender, EventArgs e)
        {
            try
            {
                _game.TryClose();
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private void _pipeHandler(object sender, ZGamePipeArgs e)
        {
            _AppendPipeContent(e.FullMessage);

            // can close game button visibility
            if (e.SecondPart.Contains(__GameRunning))
            {
                _view.SetCanClose(true);
            }
            // unfold game window
            if (e.SecondPart.Contains(__GameLoading) || e.SecondPart.Contains(__WaitForPeerClient))
            {
                var launcherSettings = _settingsService.GetLauncherSettings();
                if (launcherSettings.UnfoldGameWindow)
                    _game.TryUnfoldGameWindow();
            }
            // dll injection
            if (e.FirstPart == __GameWaiting)
            {
                if (!_settingsService.GlobalBlock)
                    _api.InjectDll(_gameSettings.Game, _gameSettings.Dlls.ToArray());

                _SetPlayingVisual();
            }

            if (_game.IsRun) return;

            // game closed
            _view.CloseClick -= _OnCloseRequestedHandler;
            _game.Pipe -= _pipeHandler;

            _view.Hide();
            _discord.DisablePlay();
            _OnWorkDone(_pipeContent);
        }

        private void _AppendPipeContent(string content)
            => _pipeContent += string.IsNullOrEmpty(_pipeContent) ? content : $"\n{content}";

        private void _SetGameVisual(BaseJoinParams param)
        {
            _SetContextVisual(param);

            _view.SetState(true);
            _view.SetCanClose(false);
        }

        protected abstract void _SetContextVisual(BaseJoinParams param);

        private void _SetPlayingVisual()
        {
            _view.SetText("Playing");
            _view.SetState(false);
        }

        #endregion
    }
}