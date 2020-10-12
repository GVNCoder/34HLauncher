using System.Windows.Input;
using Launcher.Core.Interaction;

namespace Launcher.Core.Shared
{
    public class TextDialogViewModel : BaseDialogViewModel
    {
        private bool _canSetResult = true;

        public TextDialogViewModel(string title, string content, TextDialogButtons buttons)
        {
            Title = title;
            Content = content;

            OkButtonEnable = buttons.HasFlag(TextDialogButtons.Ok);
            NoButtonEnable = buttons.HasFlag(TextDialogButtons.No);
            CancelButtonEnable = buttons.HasFlag(TextDialogButtons.Cancel);
        }

        public string Title { get; }
        public string Content { get; }

        public bool OkButtonEnable { get; }
        public bool NoButtonEnable { get; }
        public bool CancelButtonEnable { get; }

        public ICommand OkCommand => new DelegateCommand(obj => _SetDialogResult(DialogActionEnum.Primary));
        public ICommand NoCommand => new DelegateCommand(obj => _SetDialogResult(DialogActionEnum.Declined));
        public ICommand CancelCommand => new DelegateCommand(obj => _SetDialogResult(DialogActionEnum.Closed));

        private void _SetDialogResult(DialogActionEnum action)
        {
            if (!_canSetResult) return;

            _canSetResult = false;
            Dialog.SetResult<object>(null, action);
        }
    }
}