using System;
using Zlo4NET.Api.Service;

namespace Launcher.Core.Data
{
    public class RunningGame : IGame
    {
        private readonly IZRunGame _game;

        public RunningGame(IZRunGame game)
        {
            _game = game;
            _game.Pipe += (sender, e) => _OnPipeLog(e.FullMessage);
        }

        public bool IsRun => _game.IsRun;

        public bool TryClose() => _game.TryClose();

        public bool TryUnfoldWindow() => _game.TryUnfoldGameWindow();

        public event EventHandler<string> PipeLog;

        private void _OnPipeLog(string content) => PipeLog?.Invoke(this, content);
    }
}