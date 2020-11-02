using System;

namespace Launcher.Core
{
    public class EventOccuredEventArgs : EventArgs
    {
        public EventItem Event { get; }

        public EventOccuredEventArgs(EventItem item)
        {
            Event = item;
        }
    }
}