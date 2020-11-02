using System;
using System.Collections.Generic;

namespace Launcher.Core
{
    public class EventService : IEventService
    {
        private const int EVENT_LOG_SIZE = 15;
        private readonly Stack<EventItem> _events;

        public EventService()
        {
            _events = new Stack<EventItem>(EVENT_LOG_SIZE);
        }

        #region IEventService

        // return a copy of items
        public IEnumerable<EventItem> EventsStack => _events.ToArray();

        public void Event(string name, string content, EventType eventType)
        {
            // log event
            var eventItem = new EventItem
            {
                Name = name,
                Content = content,
                EventType = eventType,
                TimeCreated = DateTime.Now
            };

            _events.Push(eventItem);

            // queue overflow
            if (_events.Count > EVENT_LOG_SIZE)
            {
                // remove last item
                _events.Pop();
            }

            // raise event
            _RaiseOnEventOccured(eventItem);
        }

        public event EventHandler<EventOccuredEventArgs> EventOccured;

        #endregion

        #region Private helpers

        private void _RaiseOnEventOccured(EventItem item)
            => EventOccured?.Invoke(this, new EventOccuredEventArgs(item));

        #endregion
    }
}