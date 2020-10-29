using System;
using System.Threading.Tasks;

namespace Launcher.Core
{
    public static class BusyIndicatorServiceExtensions
    {
        public static async Task BusyWhileAsync(this IBusyIndicatorService busyIndicatorService, Action<object> heavyOperation, object state, string title = null)
        {
            // show indicator
            busyIndicatorService.Open(title);

            // start operation
            await Task.Run(() => heavyOperation.Invoke(state));

            // hide indicator
            busyIndicatorService.Close();
        }
    }
}