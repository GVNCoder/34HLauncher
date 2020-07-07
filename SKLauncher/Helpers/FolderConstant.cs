using System;

namespace Launcher.Helpers
{
    public static class FolderConstant
    {
        public const string UpdateFolder = "update";
        public const string SettingsFolder = "settings";
        public const string LogFolder = "log";
        public const string ResourcesFolder = "resources";

        public static string BaseDir { get; } = AppDomain.CurrentDomain.BaseDirectory;
    }
}