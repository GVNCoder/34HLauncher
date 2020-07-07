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

        void UpdateServerBrowser(ZGame game); // large image and Details => Browse servers and large image tooltip => full game name
        void UpdateCoopBrowser(); // large image and Details => Browse COOP missions and large image tooltip => full game name
        void UpdateStats(ZGame game); // large image and Details => View statistics and large image tooltip => full game name
        void UpdateAFK(); // large image and Details => AFK and large image tooltip => null

        #endregion

        #region Game

        void UpdateServer(ZServerBase server);
        void UpdateCoop(ZPlayMode mode, CoopMissionModel model);
        void UpdateSingle(ZGame game, ZPlayMode mode);

        #endregion
    }
}