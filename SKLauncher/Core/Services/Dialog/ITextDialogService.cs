using System.Threading.Tasks;
using Launcher.Core.Shared;

namespace Launcher.Core.Services.Dialog
{
    public interface ITextDialogService
    {
        Task<DialogResult> OpenDialog(string title, string content, TextDialogButtons buttons);
    }
}