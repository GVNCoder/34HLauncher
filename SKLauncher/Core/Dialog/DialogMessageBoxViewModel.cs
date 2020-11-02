using System.Windows.Input;
using Launcher.Core.Interaction;

namespace Launcher.Core.Dialog
{
    public class DialogMessageBoxViewModel : BaseDialogViewModel
    {
        public DialogMessageBoxViewModel(string title, string message)
        {
            Title = title;
            Message = message;
        }

        #region Bindable properties

        public string Title { get; }
        public string Message { get; }

        #endregion

        #region Commands

        public ICommand CloseCommand => new DelegateCommand(parameter => Dialog.CloseDialog(DialogAction.Cancel));

        #endregion
    }
}