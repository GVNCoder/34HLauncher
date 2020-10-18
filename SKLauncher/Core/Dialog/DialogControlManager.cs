using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Launcher.XamlThemes.Controls;

namespace Launcher.Core.Dialog
{
    public class DialogControlManager : IDialogSystemBase
    {
        private DialogControl _dialogControl;
        private UserControl _currentDialogContent;

        #region IDialogControlManager

        public Task<DialogResult?> Show<TUserControl>(BaseDialogViewModel viewModel) where TUserControl : UserControl, new ()
        {
            Task<DialogResult?> dialogTask;

            // check, can use dialog control at the moment
            if (_IsDialogControlFree()) // then pass Show() call
            {
                // create control
                var control = new TUserControl { DataContext = viewModel };
                var completionSource = new TaskCompletionSource<DialogResult?>();
                var dialog = new Dialog(completionSource, this);

                // pass dialog for remote control
                viewModel.Dialog = dialog;

                // setup control
                _dialogControl.Content = control;
                _dialogControl.IsOpen = true;

                // save task
                dialogTask = completionSource.Task;

                // save content
                _currentDialogContent = control;
            }
            else // then pass Show() call with NULL result
            {
                dialogTask = Task.FromResult<DialogResult?>(null);
            }
            
            // return task
            return dialogTask;
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
            _dialogControl.Closed += _DialogClosedCallback;

            // inject dialog control to container
            dialogContainer.Children.Add(_dialogControl);
        }

        #endregion

        #region Private helpers

        // indicates, when we can use dialog control
        private bool _IsDialogControlFree() => ! _dialogControl.IsOpen;

        private void _DialogClosedCallback(object sender, EventArgs e)
        {
            // try to exclude user control from application visual tree
            var contentPresenter = (Border) _currentDialogContent.Parent;

            // exclude
            contentPresenter.Child = null;

            _currentDialogContent.Content = null;
            _currentDialogContent = null;
        }

        #endregion
    }
}