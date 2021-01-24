using Launcher.Core.Shared;

using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Data
{
    public class CreateCoopParameters : CreateGameParametersBase
    {
        public ZPlayMode Mode { get; set; }
        public uint? FriendId { get; set; }
        public CoopMissionModel CoopMission { get; set; }
    }
}