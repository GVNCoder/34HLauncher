using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

using Launcher.Core.Data;
using Launcher.Core.Data.Model.Event;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;

namespace Launcher.Core
{
    public class VisualProvider : IVisualProvider
    {
        private const string WINDOW_BACKGROUND_CONTENT = "HOST_WindowBackground";

        private readonly IPageNavigator _navigator;
        private readonly Visual _windowContentRef;

        public VisualProvider(IPageNavigator navigator)
        {
            _navigator = navigator;

            // track content changed
            _navigator.Navigated += _ControlVisualContentChangedHandler;

            // window background content is always static
            var rootGrid = (Grid) _navigator.Container.Parent;

            foreach (var child in rootGrid.Children)
            {
                var control = (FrameworkElement)child;

                // find window background content
                // ReSharper disable once InvertIf
                if (control.Name == WINDOW_BACKGROUND_CONTENT)
                {
                    _windowContentRef = control;
                    break;
                }
            }
        }

        #region IVisualProvider

        public Visual GetVisualContent(VisualContext context)
        {
            Visual visualContent = null;

            // gets visual content by context
            switch (context)
            {
                case VisualContext.Page:

                    visualContent = _windowContentRef;
                    break;
                case VisualContext.Control:

                    // save current content
                    visualContent = _navigator.CurrentPage;
                    break;
                default: break;
            }

            return visualContent;
        }

        public event Action<VisualContentChangedEventArgs> VisualContentChanged;

        #endregion

        #region Private helper

        private void _ControlVisualContentChangedHandler(object sender, NavigationEventArgs e)
            // redirect content
            => _RaiseOnVisualContentChangedEvent((Visual) e.Content);

        private void _RaiseOnVisualContentChangedEvent(Visual content)
            => VisualContentChanged?.Invoke(new VisualContentChangedEventArgs(content));

        #endregion
    }
}