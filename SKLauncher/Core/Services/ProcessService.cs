using System.Diagnostics;
using System.IO;

namespace Launcher.Core.Services
{
    public class ProcessService : IProcessService
    {
        public bool Run(string path, bool useWorkingDirectory)
        {
            try
            {
                var startInfo = new ProcessStartInfo(path);
                if (useWorkingDirectory) startInfo.WorkingDirectory = Path.GetDirectoryName(path);

                Process.Start(startInfo);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}