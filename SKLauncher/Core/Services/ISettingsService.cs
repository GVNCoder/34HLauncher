using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public interface ISettingsService
    {
        bool Load();
        bool Save();

        void SetLock();
        void FreeLock();

        bool CanGetAccess();
        bool IsDefault();

        LauncherSettings Current { get; }
    }
}