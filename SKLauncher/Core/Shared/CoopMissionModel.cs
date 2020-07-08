using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Shared
{
    public class CoopMissionModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ZCoopLevels Level { get; set; }
        public ZCoopDifficulty Difficulty { get; set; }
    }
}