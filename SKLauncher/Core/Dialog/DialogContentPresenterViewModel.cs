using System.Windows.Input;
using Launcher.Core.Interaction;

namespace Launcher.Core.Dialog
{
    public class DialogContentPresenterViewModel : BaseDialogViewModel
    {
        public DialogContentPresenterViewModel(object content)
        {
            Content = content;
        }

        #region Bindable properties

        public object Content { get; }

        #endregion

        #region Commands

        public ICommand CloseCommand => new DelegateCommand(parameter => Dialog.CloseDialog(DialogAction.Cancel));

        #endregion
    }
}