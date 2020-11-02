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
        private const string TimeCreatedFormatString = "HH:mm:ss";

        private readonly IDiscord _discord;

        public EventLogViewModel(
            IDiscord discord,
            IEventService eventService)
        {
            _discord = discord;

            // track events
            eventService.EventOccured += _OnEventReceived;

            // build eventsList
            var eventList = eventService;

            // move logged event into current viewModel
            Events = new ObservableCollection<EventItemViewModel>(
                eventList.EventsStack
                    .Select(_BuildItemViewModelFromItem));
        }

        #region Bindable properties

        public ObservableCollection<EventItemViewModel> Events { get; }

        #endregion

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            _discord.UpdateAFK();
        });

        public override ICommand UnloadedCommand => null;

        #region Private helpers

        private void _OnEventReceived(object sender, EventOccuredEventArgs e)
        {
            _HandleIncomingEvent(e);

            // check events collection overflow
            // ReSharper disable once InvertIf
            if (Events.Count > EVENT_LOG_SIZE)
            {
                _RemoveLastEvent();
            }
        }

        private void _HandleIncomingEvent(EventOccuredEventArgs e) => Dispatcher.Invoke(() =>
        {
            // create event
            var eventItem = _BuildItemViewModelFromItem(e.Event);

            // log event
            Events.Insert(0, eventItem);
        });

        private void _RemoveLastEvent() => Dispatcher.Invoke(() =>
        {
            var lastItem = Events.Last();
            var itemIndex = Events.IndexOf(lastItem);

            Events.RemoveAt(itemIndex);
        });

        private static EventItemViewModel _BuildItemViewModelFromItem(EventItem item)
            => new EventItemViewModel
            {
                EventName = item.Name,
                Content = item.Content,
                EventType = item.EventType,
                TimeCreated = item.TimeCreated.ToString(TimeCreatedFormatString)
            };

        #endregion
    }
}