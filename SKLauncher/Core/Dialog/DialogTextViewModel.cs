using System.Windows.Input;
using Launcher.Core.Interaction;

namespace Launcher.Core.Dialog
{
    public class DialogTextViewModel : BaseDialogViewModel
    {
        public DialogTextViewModel(string title, string content, DialogButtons buttons)
        {
            Title = title;
            Content = content;

            OkButtonEnable = buttons.HasFlag(DialogButtons.Ok);
            NoButtonEnable = buttons.HasFlag(DialogButtons.No);
            CancelButtonEnable = buttons.HasFlag(DialogButtons.Cancel);
        }

        public string Title { get; }
        public string Content { get; }

        public bool OkButtonEnable { get; }
        public bool NoButtonEnable { get; }
        public bool CancelButtonEnable { get; }

        public ICommand OkCommand => new DelegateCommand(obj => Dialog.CloseDialog(DialogAction.Primary));
        public ICommand NoCommand => new DelegateCommand(obj => Dialog.CloseDialog(DialogAction.Declined));
        public ICommand CancelCommand => new DelegateCommand(obj => Dialog.CloseDialog(DialogAction.Cancel));
    }
}