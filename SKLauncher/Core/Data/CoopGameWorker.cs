using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Shared;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

namespace Launcher.Core.Data
{
    public class CoopGameWorker : BaseGameWorker
    {
        public CoopGameWorker(
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
            var convertedParam = (CoopJoinParams) param;

            switch (convertedParam.Mode)
            {
                case ZPlayMode.CooperativeHost:
                    _view.SetText("Host room");
                    _view.SetToolTipText("Host CoOp room");

                    _discord.UpdateCoop(convertedParam.Mode, convertedParam.CoopMission);

                    break;
                case ZPlayMode.CooperativeClient:
                    _view.SetText("Joining friend");
                    _view.SetToolTipText("Playing CoOp with friend");

                    _discord.UpdateCoop(convertedParam.Mode, convertedParam.CoopMission);

                    break;
            }
        }
    }
}