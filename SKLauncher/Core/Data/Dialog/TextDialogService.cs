using System.Threading.Tasks;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Shared;
using Launcher.UserControls;

namespace Launcher.Core.Data.Dialog
{
    public class TextDialogService : ITextDialogService
    {
        private readonly IOverlayDialogService _dialogService;

        public TextDialogService(IOverlayDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<DialogResult> OpenDialog(string title, string content, TextDialogButtons buttons)
        {
            var viewModel = new TextDialogViewModel(title, content, buttons);
            var dialogResult = await _dialogService.CreateDialog<TextDialogControl>(viewModel);
            return dialogResult;
        }
    }
}