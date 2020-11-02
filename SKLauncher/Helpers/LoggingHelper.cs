using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Launcher.Core.Data;
using Launcher.Core.Services;

using Ninject;

namespace Launcher.Helpers
{
    public static class LoggingHelper
    {
        #region Const

        private const int TRACE_LIMIT = 7;
        private static readonly Version VERSION;

        #endregion

        static LoggingHelper()
        {
            // resolve svc
            var versionSvc = Resolver.Kernel.Get<IVersionService>();

            // get current version
            VERSION = versionSvc.GetAssemblyVersion();
        }

        #region Private helpers

        private static StackTrace _GetTraceObject(Exception ex) => new StackTrace(ex, true);

        private static IEnumerable<StackFrame> _GetFrames(StackTrace trace) => trace.GetFrames()?.Take(TRACE_LIMIT);

        private static string _GetLogPart(StackFrame frame) => $"at [{frame.GetMethod().Name}]{Environment.NewLine}";

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

        #endregion

        public static string GetMessage(Exception exception)
        {
            var outerExceptionMessage = _GetLogForException(exception);
            var innerExceptionMessage = exception.InnerException == null
                ? string.Empty
                : $"Inner {_GetLogForException(exception.InnerException)}";
            var logMessage = $"[{VERSION}] {outerExceptionMessage} {innerExceptionMessage}";
            
            return logMessage;
        }
    }
}