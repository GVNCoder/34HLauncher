using System;
using Zlo4NET.Api.Service;

namespace Launcher.Core.Shared
{
    public class GameEventArgs : EventArgs
    {
        public GameEventType EventType { get; }
        public string PipeContent { get; }
        public GameSetting Settings { get; }
        public IZRunGame Game { get; }

        public GameEventArgs(GameEventType eventType, string pipeContent, GameSetting settings, IZRunGame game)
        {
            EventType = eventType;
            PipeContent = pipeContent;
            Settings = settings;
            Game = game;
        }
    }
}