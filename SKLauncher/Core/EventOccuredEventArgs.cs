﻿using System;

namespace Launcher.Core
{
    public class EventOccuredEventArgs : EventArgs
    {
        public string Name { get; }
        public string Content { get; }
        public EventType EventType { get; }
        public DateTime TimeCreated { get; }

        public EventOccuredEventArgs(string name, string content, EventType eventType, DateTime timeCreated)
        {
            Name = name;
            Content = content;
            EventType = eventType;
            TimeCreated = timeCreated;
        }
    }
}