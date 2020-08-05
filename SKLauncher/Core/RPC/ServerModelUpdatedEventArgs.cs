using System;
using Zlo4NET.Api.Models.Server;

namespace Launcher.Core.RPC
{
    public class ServerModelUpdatedEventArgs : EventArgs
    {
        public ZServerBase Model { get; }

        public ServerModelUpdatedEventArgs(ZServerBase model)
        {
            Model = model;
        }
    }
}