using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Launcher.Core.Bases;
using Launcher.Core.Dialog;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Services;
using Launcher.Core.Services.EventLog;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;
using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.ViewModel
{
    public class BF4ServerBrowserViewModel : BaseServerBrowserViewModel
    {
        public BF4ServerBrowserViewModel(
            IZApi api,
            IUIHostService hostService,
            IEventLogService eventLogService,
            IGameService gameService,
            IDiscord discord,
            App application,
            IPageNavigator navigator,
            ISettingsService settingsService,
            IDialogService dialogService)
            : base(api, hostService, gameService, eventLogService, discord, application, settingsService, navigator, dialogService)
        {
            MapNames = new[] { "All" }
                .Concat(ZResource.GetBF4MapNames())
                .ToArray();
            GameModeNames = new[] { "All" }
                .Concat(ZResource.GetBF4GameModeNames())
                .ToArray();
        }

        #region Overrides

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            base.OnLoadImpl(ZGame.BF4, obj as Page);
            _discord.UpdateServerBrowser(ZGame.BF4);
        });

        public override ICommand UnloadedCommand => new DelegateCommand(obj => OnUnloadedImpl());

        #endregion
    }
}