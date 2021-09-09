using System.Linq;
using System.Windows.Input;
using System.Windows.Controls;

using Launcher.Core;
using Launcher.Core.Bases;
using Launcher.Core.Dialog;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Services;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;

using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.ViewModel
{
    public class BFHServerBrowserViewModel : BaseServerBrowserViewModel
    {
        #region Ctor

        public BFHServerBrowserViewModel(
            IZApi api,
            IEventService eventService,
            IGameService gameService,
            IDiscord discord,
            App application,
            IPageNavigator navigator,
            ISettingsService settingsService,
            IDialogService dialogService,
            IBusyIndicatorService busyIndicatorService)
            : base(api, gameService, eventService, discord, application, settingsService, navigator, dialogService, busyIndicatorService)
        {
            MapNames = new[] { "All" }
                .Concat(ZResource.GetBFHMapNames())
                .ToArray();
            GameModeNames = new[] { "All" }
                .Concat(ZResource.GetBFHGameModeNames())
                .ToArray();
        }

        #endregion

        #region Overrides

        public override ICommand LoadedCommand => new DelegateCommand(obj => OnLoadImpl(ZGame.BFH, (Page) obj));

        public override ICommand UnloadedCommand => new DelegateCommand(obj => OnUnloadedImpl());

        #endregion
    }
}