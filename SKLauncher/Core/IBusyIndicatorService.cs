namespace Launcher.Core
{
    public interface IBusyIndicatorService
    {
        void Open(string title = null);
        void Close();
    }
}