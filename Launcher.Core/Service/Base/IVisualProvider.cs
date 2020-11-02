using System;
using System.Windows.Media;

using Launcher.Core.Data;
using Launcher.Core.Data.Model.Event;

namespace Launcher.Core.Service.Base
{
    /// <summary>
    /// Defines acrylic effect visual provider
    /// </summary>
    public interface IVisualProvider
    {
        /// <summary>
        /// Gets visual content for acrylic effect
        /// </summary>
        /// <param name="context">Target visual content type</param>
        /// <returns>Returns a visual content</returns>
        Visual GetVisualContent(VisualContext context);
        /// <summary>
        /// Occurs, when <see cref="VisualContext"/> is <see cref="VisualContext.Control"/> and content was changed
        /// </summary>
        event Action<VisualContentChangedEventArgs> VisualContentChanged;
    }
}