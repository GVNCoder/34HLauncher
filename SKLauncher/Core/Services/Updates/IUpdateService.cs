using System;
using System.Threading.Tasks;

using Launcher.Core.Data.Updates;
using Launcher.Core.Shared;

namespace Launcher.Core.Services.Updates
{
    public interface IUpdateService
    {
        void BeginUpdate();
        Task CancelDownload();

        bool InDownloadStage { get; }

        Func<Task<bool>> CancelDownloadResolver { get; set; }
        Func<LauncherVersion, bool> UpdateAvailableResolver { get; set; }

        event EventHandler<UpdateErrorEventArgs> Error;
        event EventHandler<UpdateProgressEventArgs> ReportProgress;
        event EventHandler<UpdateDownloadEventArgs> EndDownload;
        event EventHandler BeginDownload;
    }
}