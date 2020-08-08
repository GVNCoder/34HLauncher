namespace Launcher.Core.Service.Base
{
    /// <summary>
    /// Defines the base class for view models for Discord Presence pages.
    /// </summary>
    public abstract class DiscordPresencePageViewModel : BasePageViewModel
    {
        protected readonly IDiscord _discord;

        protected DiscordPresencePageViewModel(IDiscord discord)
        {
            _discord = discord;
        }
    }
}