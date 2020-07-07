using Launcher.Core.Shared;

namespace Launcher.Core.Services.EventLog
{
    public interface IEventLogService
    {
        EventViewModel Log(EventLogLevel level, string header, string content);
        bool HasEvents { get; }
    }
}