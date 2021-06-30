using System;
using System.Diagnostics;
using System.IO;

using Microsoft.Win32;

namespace Launcher.Helpers
{
    public static class ZClientProcessHelper
    {
        private const string REGISTRY_KEY = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\ZLO";
        private const string PROPERTY_KEY = "ZClientPath";
        private const string ZCLIENT_FILENAME = "ZClient.exe";

        public static string TryGetPathFromRegistry()
        {
            string fullPath = null;

            try
            {
                // get directory path from registry
                var directoryPath = (string) Registry.GetValue(REGISTRY_KEY, PROPERTY_KEY, string.Empty);

                // build a full path
                fullPath = Path.Combine(directoryPath, ZCLIENT_FILENAME);
            }
            catch (Exception)
            {
                // suppress
            }

            return fullPath;
        }

        public static string GetExecutionFilePath(Process process)
        {
            return process?.MainModule?.FileName;
        }
    }
}