using System;
using System.Threading.Tasks;
using Launcher.Core.Data.Updates;
using Launcher.Core.Shared;

namespace Launcher.Core.Services.Updates
{
    public interface IUpdateService
    {
        void BeginUpdate();

        bool InDownloadStage { get; }
        Func<Task<bool>> CancelDownloadResolver { get; set; }
        Func<LauncherVersion, bool> UpdateAvailableResolver { get; set; }

        event EventHandler<UpdateErrorEventArgs> Error;
    }
}