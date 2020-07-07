using Launcher.Core.Locators;
using Ninject.Modules;

namespace Launcher.Core.Injection
{
    public class LocatorsModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<ILocatorGroup>()
                .To<LocatorGroup>()
                .InSingletonScope();

            Kernel.Bind<IViewModelLocator>()
                .To<ViewModelLocator>()
                .InSingletonScope();
        }
    }
}