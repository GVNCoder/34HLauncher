using System.Windows;
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

            OkButtonVisibility = buttons.HasFlag(DialogButtons.Ok) ? Visibility.Visible : Visibility.Collapsed;
            NoButtonVisibility = buttons.HasFlag(DialogButtons.No) ? Visibility.Visible : Visibility.Collapsed;
            CancelButtonVisibility = buttons.HasFlag(DialogButtons.Cancel) ? Visibility.Visible : Visibility.Collapsed;
        }

        public string Title { get; }
        public string Content { get; }

        public Visibility OkButtonVisibility { get; }
        public Visibility NoButtonVisibility { get; }
        public Visibility CancelButtonVisibility { get; }

        public ICommand OkCommand => new DelegateCommand(obj => Dialog.CloseDialog(DialogAction.Primary));
        public ICommand NoCommand => new DelegateCommand(obj => Dialog.CloseDialog(DialogAction.Declined));
        public ICommand CancelCommand => new DelegateCommand(obj => Dialog.CloseDialog(DialogAction.Cancel));
    }
}