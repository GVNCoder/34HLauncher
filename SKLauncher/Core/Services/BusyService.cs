using System;
using System.Threading.Tasks;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Shared;
using Launcher.UserControls;

namespace Launcher.Core.Services
{
    public class BusyService : IBusyService
    {
        private readonly IOverlayDialogService _dialogService;
        private DialogControlHelper _dialogHelper;
        private bool _isShow;

        public BusyService(IOverlayDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async void Busy(string title)
        {
            if (_isShow) return;

            var viewModel = new BusyViewModel(title);
            var dialogTask = _dialogService.CreateDialog<UserBusyIndicator>(viewModel);
            _dialogHelper = viewModel.Dialog;
            _isShow = true;
            await dialogTask;
        }

        public async Task BusyWhile(string title, Action<object> whileOperation, object state)
        {
            Busy(title);
            await Task.Run(() => whileOperation.Invoke(state));
            Free();
        }

        public void Free()
        {
            if (!_isShow) return;

            _dialogHelper.Close();
            _isShow = false;
        }
    }
}