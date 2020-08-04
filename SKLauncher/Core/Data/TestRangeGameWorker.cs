using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Shared;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

namespace Launcher.Core.Data
{
    public class TestRangeGameWorker : BaseRunningGameWorker
    {
        public TestRangeGameWorker(
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
            _view.SetText("Playground");
            _view.SetToolTipText("Test range");

            _discord.UpdateSingle(param.Game, ZPlayMode.TestRange);
        }
    }
}