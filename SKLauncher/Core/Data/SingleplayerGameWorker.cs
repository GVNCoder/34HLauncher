using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Shared;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

namespace Launcher.Core.Data
{
    public class SingleplayerGameWorker : BaseGameWorker
    {
        public SingleplayerGameWorker(
            IZRunGame game,
            GameSetting gameSettings,
            BaseJoinParams param,
            IGameControl view,
            IZApi api,
            ISettingsService settingsService,
            IDiscord discord) : base(game, gameSettings, param, view, api, settingsService, discord)
        {
        }

        protected override void _SetContextVisual(BaseJoinParams param)
        {
            _view.SetText("Resume campaign");
            _view.SetToolTipText("Campaign");

            _discord.UpdateSingle(param.Game, ZPlayMode.Singleplayer);
        }
    }
}