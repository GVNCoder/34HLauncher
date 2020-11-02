using System.Windows;
using System.Windows.Controls;
using Launcher.XamlThemes.Controls;

namespace Launcher.Core
{
    public class BusyIndicatorControlManager : IBusyIndicatorBase
    {
        private const int CONTROL_POSITION = 1;

        private BusyIndicatorControl _busyIndicatorControl;

        #region IUIHostDependency

        public void SetDependency(FrameworkElement element)
        {
            // extract grid
            var controlsContainer = (Grid) element;

            // create control
            _busyIndicatorControl = new BusyIndicatorControl();

            // inject dialog control to container
            // control will be located over all dialogs
            controlsContainer.Children.Insert(CONTROL_POSITION, _busyIndicatorControl);
        }

        #endregion

        #region IBusyIndicatorBase

        public void Open(string title)
        {
            // setup control
            _busyIndicatorControl.Title = title;
            _busyIndicatorControl.IsOpen = true;
        }

        public void Close()
        {
            _busyIndicatorControl.IsOpen = false;
        }

        #endregion
    }
}