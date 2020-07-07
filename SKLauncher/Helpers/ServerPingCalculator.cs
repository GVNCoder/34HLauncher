using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zlo4NET.Api.Models.Server;
using Zlo4NET.Core.Helpers;

namespace Launcher.Helpers
{
    public static class ServerPingCalculator
    {
        public static async Task CalculateAsync(IEnumerable<ZServerBase> serversCollection)
        {
            await Task.Run(() =>
            {
                try
                {
                    foreach (var zServerBase in serversCollection)
                    {
                        zServerBase.Ping = ZPingService.GetPing(zServerBase.Ip);
                    }
                }
                catch (Exception) // invalid operation exception. Enumerable was changed
                {
                    // ignore
                }
            });
        }
    }
}