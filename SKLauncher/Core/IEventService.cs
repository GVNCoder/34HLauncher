using System;

namespace Launcher.Core
{
    public interface IEventService
    {
        void Event(string name, string content, EventType eventType);

        event EventHandler<EventOccuredEventArgs> EventOccured;
    }
}