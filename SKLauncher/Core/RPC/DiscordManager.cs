using Launcher.Core.Services;
using Launcher.Core.Shared;

using Ninject;
using Ninject.Syntax;

namespace Launcher.Core.RPC
{
    public class DiscordManager : IDiscordManager
    {
        private readonly IResolutionRoot _resolver;
        private readonly LauncherSettings _settings;

        private IDiscord _realDiscord;
        private IDiscord _proxyDiscord;

        public DiscordManager(IResolutionRoot resolver, ISettingsService settingsService)
        {
            _resolver = resolver;
            _settings = settingsService.GetLauncherSettings();
        }

        public IDiscord GetDiscordService()
        {
            return _settings.UseDiscordPresence
                ? _realDiscord ?? (_realDiscord = _resolver.Get<IDiscord>())
                : _proxyDiscord ?? (_proxyDiscord = new ProxyDiscord());
        }

        public IDiscord Service => GetDiscordService();
    }
}