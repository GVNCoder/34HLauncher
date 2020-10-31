using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Launcher.Core;
using Launcher.Core.Interaction;
using Launcher.Core.Service.Base;

using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.ViewModel
{
    public class EventLogViewModel : BasePageViewModel
    {
        private const int EVENT_LOG_SIZE = 15;

        private readonly IDiscord _discord;

        public EventLogViewModel(
            IDiscord discord,
            IEventService eventService)
        {
            _discord = discord;

            // track events
            eventService.EventOccured += _OnEventReceived;
        }

        #region Bindable properties

        public ObservableCollection<EventItemViewModel> Events { get; }
            = new ObservableCollection<EventItemViewModel>();

        #endregion

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            _discord.UpdateAFK();
        });

        public override ICommand UnloadedCommand => null;

        #region Private helpers

        private void _OnEventReceived(object sender, EventOccuredEventArgs e)
        {
            // create event
            var eventItem = new EventItemViewModel
            {
                EventName = e.Name,
                Content = e.Content,
                EventType = e.EventType
            };

            // log event
            Events.Add(eventItem);

            // check events collection overflow
            // ReSharper disable once InvertIf
            if (Events.Count > EVENT_LOG_SIZE)
            {
                var lastItem = Events.Last();
                var itemIndex = Events.IndexOf(lastItem);

                Events.RemoveAt(itemIndex);
            }
        }

        #endregion
    }
}