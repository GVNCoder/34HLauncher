using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Service
{
    /// <summary>
    /// Defines the Discord RPC service (RichPresence).
    /// </summary>
    public interface IDiscord
    {
        /// <summary>
        /// Enables a Discord presence.
        /// </summary>
        void EnablePresence();
        /// <summary>
        /// Disables a Discord presence.
        /// </summary>
        void DisablePresence();
        /// <summary>
        /// Toggles Discord between game and page.
        /// </summary>
        void TogglePresence();
        
        /// <summary>
        /// Sets AFK RP
        /// </summary>
        void SetAFKPage();
        /// <summary>
        /// Sets Server Browser page RP
        /// </summary>
        /// <param name="targetGame">The target game context</param>
        void SetServerBrowserPage(ZGame targetGame);
        /// <summary>
        /// Sets Stats page RP
        /// </summary>
        /// <param name="targetGame">The target game context</param>
        void SetStatsPage(ZGame targetGame);
        /// <summary>
        /// Sets Coop page RP
        /// </summary>
        void SetCoopPage();

        /// <summary>
        /// Sets Multiplayer game RP
        /// </summary>
        /// <param name="serverModel">The target server model context</param>
        void SetServerGame(ZServerBase serverModel);
        // void SetCoopGame(ZPlayMode mode, CoopMissionModel model); TODO: Add CoopMissionModel to project
        /// <summary>
        /// Sets Singleplayer game RP
        /// </summary>
        /// <param name="targetGame">The target game context</param>
        void SetCampaignGame(ZGame targetGame);
        /// <summary>
        /// Sets Playground game RP
        /// </summary>
        void SetTestRangeGame();
        /// <summary>
        /// Sets Unknown game RP
        /// </summary>
        void SetUnknownGame();
    }
}