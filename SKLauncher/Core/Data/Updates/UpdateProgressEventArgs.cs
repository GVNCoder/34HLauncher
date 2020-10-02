using System;

namespace Launcher.Core.Data.Updates
{
    public class UpdateProgressEventArgs : EventArgs
    {
        public int Progress { get; }

        public UpdateProgressEventArgs(int progress)
        {
            Progress = progress;
        }
    }
}