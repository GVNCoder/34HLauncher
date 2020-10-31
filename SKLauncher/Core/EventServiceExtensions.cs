namespace Launcher.Core
{
    public static class EventServiceExtensions
    {
        public static void InfoEvent(this IEventService eventService, string name, string content) =>
            eventService.Event(name, content, EventType.Info);

        public static void WarnEvent(this IEventService eventService, string name, string content) =>
            eventService.Event(name, content, EventType.Warn);

        public static void ErrorEvent(this IEventService eventService, string name, string content) =>
            eventService.Event(name, content, EventType.Error);
    }
}