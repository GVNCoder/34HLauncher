using Launcher.Core.Shared;

using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.RPC
{
    public interface IDiscord
    {
        void Start();
        void Stop();

        #region VM change

        void UpdateServerBrowser(ZGame game);
        void UpdateCoopBrowser();
        void UpdateStats(ZGame game);
        void UpdateAFK();

        #endregion

        #region Game

        void UpdateServer(ZServerBase server);
        void UpdateCoop(ZPlayMode mode, CoopMissionModel model);
        void UpdateSingle(ZGame game, ZPlayMode mode);
        void UpdateUnknown();

        void DisablePlay();

        #endregion
    }
}