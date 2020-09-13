using Launcher.Core.Data;
using Launcher.Core.Service;

using Ninject.Modules;

namespace Launcher.Core.Internal.Module
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            // Service singleton
            Kernel.Bind<IViewModelSource>().To<ViewModelSource>()
                .InSingletonScope();

            // Global instances
            
        }
    }
}