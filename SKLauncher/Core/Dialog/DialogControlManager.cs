using System.Windows;
using System.Windows.Controls;

using Launcher.Core.Helper;
using Launcher.Core.Service;

using Launcher.XamlThemes.Controls;

namespace Launcher.Core.Dialog
{
    public class DialogControlManager : IDialogControlManager
    {
        private const string __HOST_DialogContainer = "HOST_DialogContainer";
        private readonly DialogControl _dialogControl;

        public DialogControlManager(IApplicationState state)
        {
            // get dialog instance
            var window = state.Application.MainWindow;
            var dialogContainer = ReflectionHelper.GetPropertyInstance<Grid>(window, __HOST_DialogContainer);

            // create dialog control
            _dialogControl = new DialogControl { IsOpen = false, Visibility = Visibility.Collapsed };

            // inject dialog control to container
            dialogContainer.Children.Add(_dialogControl);
        }

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
    }
}