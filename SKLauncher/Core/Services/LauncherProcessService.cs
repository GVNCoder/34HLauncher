using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public class LauncherProcessService : ILauncherProcessService
    {
        private const string RESTART_FLAG = "-r";
        private const string UPDATE_FLAG = "-u";

        private readonly string _processName;
        private readonly string _updaterName;

        public LauncherProcessService()
        {
            _processName = Process.GetCurrentProcess().ProcessName;
            _updaterName = "34H Update";
        }

        public void RestartLauncher(Application currentApplication, string assemblyLocation)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = assemblyLocation,
                Arguments = RESTART_FLAG
            });

            currentApplication.Shutdown(0);
        }

        public void HandleProcessInstance(Application currentApplication, string[] commandArgs)
        {
            if (Process.GetProcessesByName(_processName).Length == 1 || (commandArgs.Length == 1 && commandArgs.Last() == RESTART_FLAG))
                return;

            currentApplication.Shutdown(0);
        }

        public void HandleUpdate(LauncherSettings settings, string[] commandArgs)
        {
            if (commandArgs.Length == 0 || commandArgs.Last() != UPDATE_FLAG) return;

            // open change log
            var fileName = $"changeLog_{settings.Localization}.txt";
            if (File.Exists(fileName)) Process.Start(fileName);
            // delete update.exe
            var uProcess = Process.GetProcessesByName(_updaterName).FirstOrDefault();
            void __delUpdate()
            {
                var uFile = $"update\\{_updaterName}.exe";
                if (File.Exists(uFile)) File.Delete(uFile);
            }

            if (uProcess != null)
            {
                uProcess.EnableRaisingEvents = true;
                uProcess.Exited += (sender, args) => __delUpdate();
            }
            else
            {
                __delUpdate();
            }
        }
    }
}