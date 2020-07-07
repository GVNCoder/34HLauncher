using System.Threading.Tasks;
using System.Windows.Controls;
using Launcher.Core.Shared;

namespace Launcher.Core.Services.Dialog
{
    public interface IOverlayDialogService
    {
        Task<DialogResult> CreateDialog<TDialogControl>(BaseDialogViewModel dialogViewModel)
            where TDialogControl : UserControl, new();
    }
}