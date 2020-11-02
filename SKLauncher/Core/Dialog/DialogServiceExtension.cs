using System.Threading.Tasks;
using System.Windows.Controls;
using Launcher.UserControls;

namespace Launcher.Core.Dialog
{
    public static class DialogServiceExtension
    {
        public static async Task OpenPresenter<TUseControl>(this IDialogService dialogService, object viewModel)
            where TUseControl : UserControl, new ()
        {
            // create user content
            var content = new TUseControl { DataContext = viewModel };
            var dialogViewModel = new DialogContentPresenterViewModel(content);

            // pass dialog
            _ = await dialogService.Show<DialogContentPresenter>(dialogViewModel);
        }

        public static Task<DialogResult?> OpenTextDialog(this IDialogService dialogService, string title,
            string text, DialogButtons buttons, bool showDontAsk = false)
        {
            // create view model
            var viewModel = new DialogTextViewModel(title, text, buttons, showDontAsk);

            // return dialog
            return dialogService.Show<DialogText>(viewModel);
        }

        public static async Task OpenMessageBox(this IDialogService dialogService, string title, string message)
        {
            // call extension method
            _ = await dialogService.OpenTextDialog(title, message, DialogButtons.Cancel);
        }
    }
}