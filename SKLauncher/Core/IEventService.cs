using System;
using System.Collections.Generic;

namespace Launcher.Core
{
    public interface IEventService
    {
        void Event(string name, string content, EventType eventType);

        event EventHandler<EventOccuredEventArgs> EventOccured;

        IEnumerable<EventItem> EventsStack { get; }
    }
}