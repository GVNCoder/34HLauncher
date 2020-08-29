using Launcher.Core.Service;
using Launcher.Helpers;

namespace Launcher.Core
{
    public static class ApplicationStateExtension
    {
        public static void RegisterVars(this IApplicationState state)
        {
            state.RegisterState(Constants.BF3_STATS, null);
            state.RegisterState(Constants.BF4_STATS, null);
            state.RegisterState(Constants.BFH_STATS, null);
            state.RegisterState(Constants.ZCLIENT_CONNECTION, true, true);
            state.RegisterState(Constants.ZCLIENT_IS_RUN, true, true);
        }
    }
}