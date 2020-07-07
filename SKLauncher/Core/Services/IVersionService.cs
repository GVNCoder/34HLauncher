using System;
using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public interface IVersionService
    {
        Version GetAssemblyVersion();
        LauncherVersion GetLauncherVersion();
    }
}