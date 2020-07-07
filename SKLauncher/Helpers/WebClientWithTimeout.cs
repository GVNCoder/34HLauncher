using System;
using System.Net;
using System.Threading.Tasks;

namespace Launcher.Helpers
{
    public class WebClientWithTimeout : WebClient
    {
        public int Timeout { get; set; } = 5000;

        public new async Task<string> DownloadStringTaskAsync(Uri address)
        {
            var t = base.DownloadStringTaskAsync(address);
            if (await Task.WhenAny(t, Task.Delay(Timeout)) != t) // time out!
            {
                CancelAsync(); // will throw "request aborted" exception, you will need to handle it
            }
            return await t;
        }
    }
}