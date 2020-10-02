using System;

namespace Launcher.Core.Data.Updates
{
    public class UpdateDownloadEventArgs : EventArgs
    {
        public bool IsCanceled { get; }

        public UpdateDownloadEventArgs(bool isCanceled)
        {
            IsCanceled = isCanceled;
        }
    }
}