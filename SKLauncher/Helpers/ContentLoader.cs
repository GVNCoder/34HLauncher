using System;
using System.Net;
using System.Threading.Tasks;

namespace Launcher.Helpers
{
    public static class ContentLoader
    {
        public static async Task<string> GetContentAsync(Uri address)
        {
            string contentString;
            using (var client = new WebClientWithTimeout())
            {
                contentString = await client.DownloadStringTaskAsync(address);
            }

            return contentString;
        }
    }
}