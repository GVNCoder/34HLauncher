using Launcher.Core.Locators;
using Ninject;

namespace Launcher.Core.Injection
{
    public class DependencyResolver : IDependencyResolver
    {
        public DependencyResolver()
        {
            Resolver = new StandardKernel(
                new ConstantsModule(),
                new ServiceModule(),
                new LocatorsModule());
        }

        public IKernel Resolver { get; }
        public ILocatorGroup Locators => Resolver.Get<ILocatorGroup>();
    }
}