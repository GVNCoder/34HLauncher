using System.Windows;
using log4net;
using Launcher.Core.Services;
using Ninject.Modules;
using Zlo4NET.Api;
using Zlo4NET.Core.Data;

namespace Launcher.Core.Injection
{
    public class ConstantsModule : NinjectModule
    {
        public override void Load()
        {
            var application = (App) Application.Current;

            Kernel.Bind<IZApi>()
                .ToConstant(ZApi.Instance);
            Kernel.Bind<ILauncherProcessService>()
                .ToConstant(application.ProcessService);
            Kernel.Bind<ILog>()
                .ToConstant(LogManager.GetLogger(typeof(App)));
            Kernel.Bind<App>()
                .ToConstant(application);
        }
    }
}