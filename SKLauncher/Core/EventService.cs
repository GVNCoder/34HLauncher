using System;

namespace Launcher.Core
{
    public class EventService : IEventService
    {
        #region IEventService

        public void Event(string name, string content, EventType eventType)
        {
            // raise event
            _RaiseOnEventOccured(name, content, eventType);
        }

        public event EventHandler<EventOccuredEventArgs> EventOccured;

        #endregion

        #region Private helpers

        private void _RaiseOnEventOccured(string name, string content, EventType type)
            => EventOccured?.Invoke(this, new EventOccuredEventArgs(name, content, type, DateTime.Now));

        #endregion
    }
}