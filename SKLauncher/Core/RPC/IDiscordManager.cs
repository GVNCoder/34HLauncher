namespace Launcher.Core.RPC
{
    public interface IDiscordManager
    {
        IDiscord GetDiscordService();

        IDiscord Service { get; }
    }
}