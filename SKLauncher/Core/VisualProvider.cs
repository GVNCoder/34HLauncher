using System.Windows.Media;

using Launcher.Core.Data;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;

namespace Launcher.Core
{
    public class VisualProvider : IVisualProvider
    {
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
                case VisualContext.Page: visualContent = (Visual) _navigator.Container.Parent;
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