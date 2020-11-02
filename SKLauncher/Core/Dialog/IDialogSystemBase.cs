using System.Threading.Tasks;
using System.Windows.Controls;

using Launcher.Core.Service.Base;

namespace Launcher.Core.Dialog
{
    public interface IDialogSystemBase : IUIHostDependency
    {
        Task<DialogResult?> Show<TUserControl>(BaseDialogViewModel viewModel) where TUserControl : UserControl, new();
        void Close();
    }
}