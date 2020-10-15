using System.Threading.Tasks;
using System.Windows.Controls;

namespace Launcher.Core.Dialog
{
    public class DialogService : IDialogService
    {
        private readonly IDialogSystemBase _dialogSystem;

        public DialogService(IDialogSystemBase dialogSystemBase)
        {
            _dialogSystem = dialogSystemBase;
        }

        #region IDialogService

        public Task<DialogResult?> Show<TUserControl>(BaseDialogViewModel viewModel)
            where TUserControl : UserControl, new()
        {
            return _dialogSystem.Show<TUserControl>(viewModel);
        }

        #endregion
    }
}