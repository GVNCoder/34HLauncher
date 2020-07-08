using System.Windows;
using System.Windows.Input;
using Launcher.Core.RPC;

namespace Launcher.Core.Bases
{
    public abstract class PageViewModelBase : DependencyObject
    {
        protected readonly IDiscord _discord;

        protected PageViewModelBase(IDiscord discord)
        {
            _discord = discord;
        }

        public abstract ICommand LoadedCommand { get; }
        public abstract ICommand UnloadedCommand { get; }
    }
}