using System;
using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Shared;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

namespace Launcher.Core.Data
{
    public class RunningGameImpl : IGameHelper
    {
        private const string __GameLoading = "GameLoading";
        private const string __WaitForPeerClient = "WaitForPeerClient";
        private const string __GameWaiting = "GameWaiting";
        private const string __GameRunning = "GameRunning";
        private const string __GameError = "Error";

        private readonly ISettingsService _settingsService;
        private readonly GameSetting _gameSettings;
        private readonly IGameControl _view;
        private readonly IDiscord _discord;
        private readonly IZRunGame _game;
        private readonly IZApi _api;

        private string _pipeContent;

        public RunningGameImpl(
            IZRunGame game,
            GameSetting gameSettings,
            RunContext context,
            IGameControl view,
            IZApi api,
            ISettingsService settingsService,
            IDiscord discord)
        {
            _settingsService = settingsService;
            _gameSettings = gameSettings;
            _discord = discord;
            _game = game;
            _view = view;
            _api = api;

            _view.CloseClick += _OnCloseRequestedHandler;

            _game.Pipe += _pipeHandler;
            _SetGameVisual(context);
        }

        public void BeginWork()
        {
            _view.Show();
        }

        public event EventHandler<GameCloseEventArgs> Close;

        #region Private methods

        private void _OnClose(string pipeContent) => Close?.Invoke(this, new GameCloseEventArgs(pipeContent));

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

            if (e.SecondPart.Contains(__GameRunning))
            {
                _view.SetCanClose(true);
            }

            if (e.SecondPart.Contains(__GameLoading) || e.SecondPart.Contains(__WaitForPeerClient))
            {
                var launcherSettings = _settingsService.GetLauncherSettings();
                if (launcherSettings.UnfoldGameWindow) _game.TryUnfoldGameWindow();
            }

            if (e.FirstPart == __GameWaiting)
            {
                if (!_settingsService.GlobalBlock)
                    _api.InjectDll(_gameSettings.Game, _gameSettings.Dlls.ToArray());

                _SetPlayingVisual();
            }

            if (_game.IsRun) return;

            _view.Hide();
            _view.CloseClick -= _OnCloseRequestedHandler;

            _game.Pipe -= _pipeHandler;
            _discord.DisablePlay();
            _OnClose(_pipeContent);
        }

        private void _AppendPipeContent(string content)
            => _pipeContent += string.IsNullOrEmpty(_pipeContent) ? content : $"\n{content}";

        private void _SetGameVisual(RunContext context)
        {
            _SetContextVisual(context);

            _view.SetState(true);
            _view.SetCanClose(false);
        }

        private void _SetContextVisual(RunContext context)
        {
            switch (context.Mode)
            {
                case ZPlayMode.Singleplayer:
                    _view.SetText("Resume campaign");
                    _view.SetToolTipText("Campaign");

                    _discord.UpdateSingle(context.Target, ZPlayMode.Singleplayer);

                    break;
                case ZPlayMode.Multiplayer:
                    var server = context.Server;

                    _view.SetText("Joining server");
                    _view.SetToolTipText($"{server.Name} | {server.CurrentMap.Name} | {server.CurrentMap.GameModeName}");

                    _discord.UpdateServer(server);

                    break;
                case ZPlayMode.CooperativeHost:
                    _view.SetText("Host room");
                    _view.SetToolTipText("Host CoOp room");

                    _discord.UpdateCoop(context.Mode, context.Mission);

                    break;
                case ZPlayMode.CooperativeClient:
                    _view.SetText("Joining friend");
                    _view.SetToolTipText("Playing CoOp with friend");

                    _discord.UpdateCoop(context.Mode, context.Mission);

                    break;
                case ZPlayMode.TestRange:
                    _view.SetText("Playground");
                    _view.SetToolTipText("Test range");

                    _discord.UpdateSingle(context.Target, ZPlayMode.TestRange);

                    break;
            }
        }

        private void _SetPlayingVisual()
        {
            _view.SetText("Playing");
            _view.SetState(false);
        }

        #endregion
    }
}