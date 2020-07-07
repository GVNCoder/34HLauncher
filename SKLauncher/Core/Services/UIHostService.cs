using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public class UIHostService : IUIHostService
    {
        private FrameworkElement[] _getHostObjects(object instance)
        {
            return instance
                .GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.Name.StartsWith(UIElementConstants.Host))
                .Select(f => f.GetValue(instance))
                .OfType<FrameworkElement>()
                .ToArray();
        }

        public FrameworkElement GetHostElement(string hostName)
        {
            var instance = Application.Current.MainWindow;
            var hostElements = _getHostObjects(instance);

            return hostElements
                .FirstOrDefault(h => h.Name.Equals(hostName));
        }

        public Panel GetHostContainer(string hostName)
        {
            var instance = Application.Current.MainWindow;
            var hostElements = _getHostObjects(instance);

            return hostElements
                .OfType<Panel>()
                .FirstOrDefault(h => h.Name.Equals(hostName));
        }

        public FrameworkElement GetHostElement(string hostName, object instance)
        {
            var hostInstances = _getHostObjects(instance);

            return hostInstances
                .FirstOrDefault(i => i.Name.Equals(hostName));
        }
    }
}