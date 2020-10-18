using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Shared;

using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.ViewModel
{
    public class EventLogViewModel : BasePageViewModel
    {
        private readonly IPageNavigator _navigator;
        private readonly IDiscord _discord;
        private readonly IApplicationState _state;

        public ObservableCollection<EventViewModel> Events { get; }

        public EventLogViewModel(
            IDiscord discord,
            IPageNavigator navigator,
            IApplicationState state)
        {
            _navigator = navigator;
            _discord = discord;
            _state = state;

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