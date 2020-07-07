using System.Windows;
using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public interface ILauncherProcessService
    {
        void RestartLauncher(Application currentApplication, string assemblyLocation);
        void HandleProcessInstance(Application currentApplication, string[] commandArgs);
        void HandleUpdate(LauncherSettings settings, string[] commandArgs);
    }
}