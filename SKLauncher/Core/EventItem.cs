using System;

namespace Launcher.Core
{
    public class EventItem
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime TimeCreated { get; set; }
        public EventType EventType { get; set; }
    }
}