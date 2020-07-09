using System;

namespace Launcher.Core.Data
{
    public class GameCloseEventArgs : EventArgs
    {
        public string PipeContent { get; }

        public GameCloseEventArgs(string pipeContent)
        {
            PipeContent = pipeContent;
        }
    }
}