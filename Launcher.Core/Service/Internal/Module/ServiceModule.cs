using Launcher.Core.Data;
using Ninject.Modules;

namespace Launcher.Core.Service.Internal.Module
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            // Service singleton
            Kernel.Bind<IViewModelSource>().To<ViewModelSource>()
                .InSingletonScope();

            // Global instances
            Kernel.Bind<IResolver>().ToConstant(Resolver.GetInstance);
        }
    }
}