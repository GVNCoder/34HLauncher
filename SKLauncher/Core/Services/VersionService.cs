using System;
using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public class VersionService : IVersionService
    {
        private static LauncherVersion _launcherVersion;

        public static void SetVersion(LauncherVersion versionInstance)
        {
            _launcherVersion = versionInstance;
        }

        public Version GetAssemblyVersion()
        {
            return _launcherVersion.AssemblyVersion;
        }

        public LauncherVersion GetLauncherVersion()
        {
            return _launcherVersion;
        }
    }
}