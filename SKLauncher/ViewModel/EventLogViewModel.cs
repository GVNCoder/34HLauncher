using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Shared;

using IBlurredPage = Launcher.Core.Bases.IBlurredPage;
using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.ViewModel
{
    public class EventLogViewModel : BasePageViewModel, IBlurredPage
    {
        private readonly IPageNavigator _navigator;
        private readonly IDiscord _discord;

        public ObservableCollection<EventViewModel> Events { get; }
        public Grid BackgroundContent { get; }

        public EventLogViewModel(
            IUIHostService hostService,
            IDiscord discord,
            IPageNavigator navigator)
        {
            _navigator = navigator;
            _discord = discord;

            BackgroundContent = hostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;
            Events = new ObservableCollection<EventViewModel>();
        }

        public ICommand CleanupEventsCommand => new DelegateCommand(obj =>
        {
            Events.Clear();

            _navigator.NavigateBack();
        });

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            _discord.UpdateAFK();
        });

        public override ICommand UnloadedCommand => null;
    }
}