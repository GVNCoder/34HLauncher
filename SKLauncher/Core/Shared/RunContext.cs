using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Shared
{
    public class RunContext
    {
        public ZGame Target { get; set; }
        public ZRole Role { get; set; }
        public ZPlayMode Mode { get; set; }

        public uint? ServerId { get; set; }
        public uint? FriendId { get; set; }

        public ZCoopDifficulty? Difficulty { get; set; }
        public ZCoopLevels? Level { get; set; }

        public ZServerBase Server { get; set; }
        public CoopMissionModel Mission { get; set; }
    }
}