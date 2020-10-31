using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Launcher.Core;
using Launcher.Core.Bases;
using Launcher.Core.Dialog;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Services;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;
using Zlo4NET.Api;

using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.ViewModel
{
    public class BF3ServerBrowserViewModel : BaseServerBrowserViewModel
    {
        public BF3ServerBrowserViewModel(
            IZApi api,
            IEventService eventService,
            IGameService gameService,
            IDiscord discord,
            App application,
            IPageNavigator navigator,
            ISettingsService settingsService,
            IDialogService dialogService)
            : base(api, gameService, eventService, discord, application, settingsService, navigator, dialogService)
        {
            MapNames = new [] { "All" }
                .Concat(ZResource.GetBF3MapNames())
                .ToArray();
            GameModeNames = new [] { "All" }
                .Concat(ZResource.GetBF3GameModeNames())
                .ToArray();
        }

        #region Overrides

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            base.OnLoadImpl(ZGame.BF3, obj as Page);
            _discord.UpdateServerBrowser(ZGame.BF3);
        });

        public override ICommand UnloadedCommand => new DelegateCommand(obj => OnUnloadedImpl());

        #endregion
    }
}