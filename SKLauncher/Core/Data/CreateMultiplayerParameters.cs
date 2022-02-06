using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Data
{
    public class CreateMultiplayerParameters : CreateGameParametersBase
    {
        public ZServerBase ServerModel { get; set; }
        public ZRole PlayerRole { get; set; }
        public string ServerPassword { get; set; }
    }
}