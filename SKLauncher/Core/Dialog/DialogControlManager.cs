using System.Windows;
using System.Windows.Controls;

using Launcher.XamlThemes.Controls;

namespace Launcher.Core.Dialog
{
    public class DialogControlManager : IDialogSystemBase
    {
        private DialogControl _dialogControl;

        #region IDialogControlManager

        public void Show(UserControl content)
        {
            // setup control
            _dialogControl.Content = content;
            _dialogControl.IsOpen = true;
        }

        public void Close()
        {
            _dialogControl.IsOpen = false;
        }

        #endregion

        #region IUIHostDependency

        public void SetDependency(FrameworkElement element)
        {
            // extract grid
            var dialogContainer = (Grid) element;

            // create dialog control
            _dialogControl = new DialogControl();

            // inject dialog control to container
            dialogContainer.Children.Add(_dialogControl);
        }

        #endregion
    }
}