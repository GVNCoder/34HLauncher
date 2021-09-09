using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

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
    public class BF4ServerBrowserViewModel : BaseServerBrowserViewModel
    {
        #region Ctor

        public BF4ServerBrowserViewModel(
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
                .Concat(ZResource.GetBF4MapNames())
                .ToArray();
            GameModeNames = new[] { "All" }
                .Concat(ZResource.GetBF4GameModeNames())
                .ToArray();
        }

        #endregion

        #region Overrides

        public override ICommand LoadedCommand => new DelegateCommand(obj => OnLoadImpl(ZGame.BF4, (Page)obj));

        public override ICommand UnloadedCommand => new DelegateCommand(obj => OnUnloadedImpl());

        #endregion
    }
}