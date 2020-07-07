using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public interface ISettingsService
    {
        bool LoadLauncherSettings();
        bool LoadGameSettings();

        LauncherSettings GetLauncherSettings();
        GameSettings GetGameSettings();

        bool Save();

        bool GlobalBlock { get; }

        void SetGlobalLock();
        void FreeGlobalLock();
    }
}