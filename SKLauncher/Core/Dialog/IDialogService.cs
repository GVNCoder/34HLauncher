using System.Threading.Tasks;
using System.Windows.Controls;

namespace Launcher.Core.Dialog
{
    public interface IDialogService
    {
        Task<DialogResult?> Show<TUserControl>(BaseDialogViewModel viewModel) where TUserControl : UserControl, new();
    }
}