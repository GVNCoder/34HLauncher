using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Launcher.Helpers
{
    public static class LoggingHelper
    {
        private const int TRACE_LIMIT = 7;

        private static StackTrace _GetTraceObject(Exception ex) => new StackTrace(ex, true);

        private static IEnumerable<StackFrame> _GetFrames(StackTrace trace) => trace.GetFrames()?.Take(TRACE_LIMIT);

        private static string _GetLogPart(StackFrame frame) =>
            $"at [{Path.GetFileName(frame.GetFileName())}][{frame.GetFileLineNumber()}][{frame.GetMethod().Name}]{Environment.NewLine}";

        private static string _GetStackTraceMessage(Exception ex)
        {
            var trace = _GetTraceObject(ex);
            var frames = _GetFrames(trace);
            var stackTrace = frames?.Aggregate(string.Empty, (current, stackFrame) => current + _GetLogPart(stackFrame));

            return stackTrace;
        }

        private static string _GetLogForException(Exception exception)
        {
            var stackTrace = _GetStackTraceMessage(exception);
            var logMessage = $"Exception message: {exception.Message}{Environment.NewLine}{stackTrace}";

            return logMessage;
        }

        public static string GetMessage(Exception exception)
        {
            var outerExceptionMessage = _GetLogForException(exception);
            var innerExceptionMessage = exception.InnerException == null
                ? string.Empty
                : $"Inner {_GetLogForException(exception.InnerException)}";
            var logMessage = $"{outerExceptionMessage}{innerExceptionMessage}";
            
            return logMessage;
        }
    }
}