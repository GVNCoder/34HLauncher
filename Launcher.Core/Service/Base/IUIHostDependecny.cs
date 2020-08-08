using System.Windows;

namespace Launcher.Core.Service.Base
{
    /// <summary>
    /// Defines an element that is dependent on UI initialization.
    /// </summary>
    public interface IUIHostDependency
    {
        /// <summary>
        /// Initializes an element with a UI element.
        /// </summary>
        /// <param name="element">The UI element (host).</param>
        void Init(FrameworkElement element);
    }
}