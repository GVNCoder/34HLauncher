using System.Windows;

namespace Launcher.Core.Service.Base
{
    /// <summary>
    /// Defines an interface for creating a blurred page background.
    /// </summary>
    public interface IBlurredPage
    {
        /// <summary>
        /// Gets visual content element instance.
        /// </summary>
        FrameworkElement VisualContentElement { get; }
    }
}