using Launcher.Core.Locators;
using Ninject;

namespace Launcher.Core.Injection
{
    public interface IDependencyResolver
    {
        IKernel Resolver { get; }
        ILocatorGroup Locators { get; }
    }
}