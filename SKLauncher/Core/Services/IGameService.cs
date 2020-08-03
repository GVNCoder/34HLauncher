using System.Threading.Tasks;
using Launcher.Core.Shared;
using Zlo4NET.Api.Models.Server;

namespace Launcher.Core.Services
{
    public interface IGameService
    {
        Task Run(RunContext context);
        void TryDetect();
    }
}