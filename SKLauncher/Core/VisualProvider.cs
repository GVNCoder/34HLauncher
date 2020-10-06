using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Launcher.Core.Data;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;

namespace Launcher.Core
{
    public class VisualProvider : IVisualProvider
    {
        private const string WINDOW_BACKGROUND_CONTENT = "HOST_WindowBackground";
        private readonly IPageNavigator _navigator;

        public VisualProvider(IPageNavigator navigator)
        {
            _navigator = navigator;
        }

        #region IVisualProvider

        public Visual GetVisualContent(VisualContext context)
        {
            Visual visualContent = null;

            // gets visual content by context
            switch (context)
            {
                case VisualContext.Page:
                    var rootGrid = (Grid) _navigator.Container.Parent;
                    foreach (var child in rootGrid.Children)
                    {
                        var control = (FrameworkElement) child;

                        // find window background content
                        // ReSharper disable once InvertIf
                        if (control.Name == WINDOW_BACKGROUND_CONTENT)
                        {
                            visualContent = control;
                            break;
                        }
                    }

                    break;
                case VisualContext.Control: visualContent = _navigator.CurrentPage;
                    break;
                default: break;
            }

            return visualContent;
        }

        #endregion
    }
}