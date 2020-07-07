using System.Windows;
using System.Windows.Controls;

namespace Launcher.Core.Services
{
    public interface IUIHostService
    {
        Panel GetHostContainer(string hostName);
        FrameworkElement GetHostElement(string hostName);

        FrameworkElement GetHostElement(string hostName, object instance);
    }
}