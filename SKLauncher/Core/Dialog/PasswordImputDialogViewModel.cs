using Launcher.Core.Interaction;

using System.Windows.Controls;
using System.Windows.Input;

namespace Launcher.Core.Dialog
{
    public class PasswordImputDialogViewModel : BaseDialogViewModel
    {
        public PasswordImputDialogViewModel(string title, string content)
        {
            Title = title;
            Content = content;
        }

        public string Title { get; }
        public string Content { get; }

        public ICommand OkCommand => new DelegateCommand(obj =>
        {
            var passwordBox = (PasswordBox) obj;
            var password = passwordBox.Password;

            Dialog.CloseDialog(DialogAction.Primary, password);
        });

        public ICommand CancelCommand => new DelegateCommand(obj => Dialog.CloseDialog(DialogAction.Cancel, null));
    }
}
