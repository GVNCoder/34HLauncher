namespace Launcher.Core.Services
{
    public interface IProcessService
    {
        bool Run(string path, bool useWorkingDirectory);
    }
}