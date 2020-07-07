using System;
using System.Reflection;

namespace Launcher.Core.Shared
{
    public sealed class LauncherVersion
    {
        public Version AssemblyVersion { get; }

        public string Candidature { get; }

        public int Major => AssemblyVersion.Major;

        public int Minor => AssemblyVersion.Minor;

        public int Build => AssemblyVersion.Build;

        public LauncherVersion(string candidature)
        {
            AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Candidature = candidature;
        }

        public LauncherVersion(int major, int minor, int build, int revision, string candidature)
        {
            AssemblyVersion = new Version(major, minor, build, revision);
            Candidature = candidature;
        }

        public static LauncherVersion Parse(string str)
        {
            var strParts = str.Split(new[] { '.', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return new LauncherVersion(
                int.Parse(strParts[0]),
                int.Parse(strParts[1]),
                int.Parse(strParts[2]),
                int.Parse(strParts[3]),
                strParts[4]);
        }

        public override string ToString()
        {
            return $"{AssemblyVersion} {Candidature}";
        }
    }
}