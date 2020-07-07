using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using Launcher.Core.Bases;
using Launcher.Core.Interaction;
using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Services.EventLog;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;
using Zlo4NET.Api;

namespace Launcher.ViewModel
{
    public class BF3ServerBrowserViewModel : BaseServerBrowserViewModel
    {
        public BF3ServerBrowserViewModel(
            IZApi api,
            IUIHostService hostService,
            IContentPresenterService presenterService,
            IEventLogService eventLogService,
            IGameService gameService,
            IDiscordManager discord)
            : base(api, hostService, gameService, eventLogService, presenterService, discord)
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