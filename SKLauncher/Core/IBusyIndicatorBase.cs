using Launcher.Core.Service.Base;

namespace Launcher.Core
{
    public interface IBusyIndicatorBase : IUIHostDependency
    {
        void Open(string title);
        void Close();
    }
}