using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Data
{
    public class MultiplayerJoinParams : BaseJoinParams
    {
        public ZServerBase ServerModel { get; set; }
        public ZRole PlayerRole { get; set; }
    }
}