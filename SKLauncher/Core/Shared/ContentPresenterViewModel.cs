using System.Windows.Input;
using Launcher.Core.Interaction;
using Launcher.Helpers;

namespace Launcher.Core.Shared
{
    public class ContentPresenterViewModel : BaseDialogViewModel
    {
        private bool _canClose = true;

        public ContentPresenterViewModel(object content)
        {
            Content = content;
        }

        public object Content { get; }

        public ICommand CloseCommand => new DelegateCommand(obj =>
        {
            if (!_canClose) return;

            _canClose = false;
            Dialog.Close();
        });
    }
}