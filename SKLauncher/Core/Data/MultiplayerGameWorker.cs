using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Shared;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Service;

namespace Launcher.Core.Data
{
    public class MultiplayerGameWorker : BaseRunningGameWorker
    {
        private ServerModelUpdatesUnit _updateServerUnit;

        public MultiplayerGameWorker(
            IZRunGame game,
            GameSetting gameSettings,
            BaseJoinParams param,
            IGameControl view,
            IZApi api,
            ISettingsService settingsService,
            IDiscord discord) : base(game, gameSettings, param, view, api, settingsService, discord)
        {
            Done += _DoneHandler;
        }

        public void RelinkServer(ZServerBase model)
        {
            var currentMap = model.MapRotation.Current;

            _discord.UpdateServer(model);
            _view.SetToolTipText($"{model.Name} | {currentMap.Name} | {currentMap.GameModeName}");
            _updateServerUnit.Relink(model);
        }

        private void _DoneHandler(object sender, GameCloseEventArgs e)
        {
            Done -= _DoneHandler;
            _updateServerUnit.Destroy();
        }

        protected override void _SetContextVisual(BaseJoinParams param)
        {
            var convertedParam = (MultiplayerJoinParams) param;
            var server = convertedParam.ServerModel;
            var currentMap = server.MapRotation.Current;

            _view.SetText("Joining server");
            _view.SetToolTipText($"{server.Name} | {currentMap.Name} | {currentMap.GameModeName}");

            _discord.UpdateServer(server);

            _updateServerUnit = new ServerModelUpdatesUnit(server);
            _updateServerUnit.ServerModelUpdated += (sender, e) =>
            {
                _view.SetToolTipText($"{e.Model.Name} | {e.Model.MapRotation.Current.Name} | {e.Model.MapRotation.Current.GameModeName}");
            };
        }
    }
}