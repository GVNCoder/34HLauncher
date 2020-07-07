using System.Windows;
using System.Windows.Input;
using Launcher.Core.RPC;

namespace Launcher.Core.Bases
{
    public abstract class PageViewModelBase : DependencyObject
    {
        protected readonly IDiscordManager _discordManager;

        protected IDiscord _discord;

        protected PageViewModelBase(IDiscordManager discordManager)
        {
            _discordManager = discordManager;
        }

        protected void OnLoadedImpl()
        {
            _discord = _discordManager.GetDiscordService();
        }

        public abstract ICommand LoadedCommand { get; }
        public abstract ICommand UnloadedCommand { get; }
    }
}