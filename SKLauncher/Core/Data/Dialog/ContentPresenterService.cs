using System.Threading.Tasks;
using System.Windows.Controls;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Shared;
using Launcher.UserControls;

namespace Launcher.Core.Data.Dialog
{
    public class ContentPresenterService : IContentPresenterService
    {
        private readonly IOverlayDialogService _dialogService;

        public ContentPresenterService(IOverlayDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task Show<TUserContent>(object viewModel) where TUserContent : UserControl, new()
        {
            var userContent = new TUserContent { DataContext = viewModel };
            var dialogViewModel = new ContentPresenterViewModel(userContent);
            _ = await _dialogService.CreateDialog<ContentPresenterControl>(dialogViewModel);
        }
    }
}