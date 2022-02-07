using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Launcher.XamlThemes.Controls;

namespace Launcher.Core.Dialog
{
    public class DialogControlManager : IDialogSystemBase
    {
        private const int QUEUE_SIZE = 10;
        private readonly Queue<_InternalDialog> _dialogQueue;

        private DialogControl _dialogControl;
        private UserControl _currentDialogContent;

        public DialogControlManager()
        {
            _dialogQueue = new Queue<_InternalDialog>(QUEUE_SIZE);
        }

        #region IDialogControlManager

        public Task<DialogResult> Show<TUserControl>(BaseDialogViewModel viewModel) where TUserControl : UserControl, new ()
        {
            // create control
            var control = new TUserControl { DataContext = viewModel };
            var completionSource = new TaskCompletionSource<DialogResult>();
            var dialog = new Dialog(completionSource, this);

            // pass dialog for remote control
            viewModel.Dialog = dialog;

            // ReSharper disable once InvertIf
            if (! _IsDialogControlFree() || _dialogQueue.Count != 0)
            {
                // add dialog content to show queue
                var internalDialog = new _InternalDialog { Content = control };
                _dialogQueue.Enqueue(internalDialog);

#if DEBUG
                Console.WriteLine($@"{nameof(IDialogSystemBase)}.{nameof(Show)}.queueCount={_dialogQueue.Count}");

                // queue overflow
                if (_dialogQueue.Count > QUEUE_SIZE)
                {
                    throw new Exception("Dialog queue overflow!");
                }
#else
#endif
            }
            else
            {
                // initial setup
                _SetupDialog(control);
            }

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
            // and move dialog queue
            var contentPresenter = (Border) _currentDialogContent.Parent;

            // exclude
            contentPresenter.Child = null;

            _currentDialogContent.Content = null;
            _currentDialogContent = null;

            // try move queue
            // ReSharper disable once InvertIf
            if (_dialogQueue.Any())
            {
                var internalDialog = _dialogQueue.Dequeue();

                // move next
                _SetupDialog(internalDialog.Content);
            }
        }

        private void _SetupDialog(UserControl content)
        {
            _currentDialogContent = content;

            // setup control
            _dialogControl.Content = content;
            _dialogControl.IsOpen = true;
        }

        #endregion
    }
}