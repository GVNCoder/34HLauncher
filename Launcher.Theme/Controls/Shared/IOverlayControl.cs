using System;

namespace Launcher.XamlThemes.Controls.Shared
{
    /// <summary>
    /// Defines overlay control.
    /// </summary>
    public interface IOverlayControl
    {
        /// <summary>
        /// Occurs, when overlay control was fully closed.
        /// </summary>
        event EventHandler Closed;
        /// <summary>
        /// Occurs, when overlay control was fully showed.
        /// </summary>
        event EventHandler Showed;
        /// <summary>
        /// Shows overlay content.
        /// </summary>
        /// <param name="content">The main content.</param>
        void Show(object content);
        /// <summary>
        /// Hides overlay control.
        /// </summary>
        void Hide();
    }
}