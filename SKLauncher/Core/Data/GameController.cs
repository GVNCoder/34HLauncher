using System;

namespace Launcher.Core.Data
{
    public class GameController
    {
        private readonly IGame _game;

        public GameController(IGame game)
        {
            _game = game;
        }

        public event EventHandler ProcessKill;
    }
}