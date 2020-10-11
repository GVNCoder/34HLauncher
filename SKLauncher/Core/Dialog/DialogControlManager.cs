using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Launcher.XamlThemes.Controls;

namespace Launcher.Core.Dialog
{
    public class DialogControlManager : IDialogSystemBase
    {
        private DialogControl _dialogControl;

        #region IDialogControlManager

        public Task<DialogResult> Show<TUserControl>(BaseDialogViewModel viewModel) where TUserControl : UserControl, new ()
        {
            // create control
            var control = new TUserControl { DataContext = viewModel };
            var completionSource = new TaskCompletionSource<DialogResult>();
            var dialog = new Dialog(completionSource, this);

            // pass dialog for remote control
            viewModel.Dialog = dialog;

            // setup control
            _dialogControl.Content = control;
            _dialogControl.IsOpen = true;

            // return task
            return completionSource.Task;
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