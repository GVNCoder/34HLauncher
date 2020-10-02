using Launcher.Data;
using Ninject;
using Ninject.Modules;

namespace Launcher.Core.Data
{
    /// <summary>
    /// Defines a class that creates and initializes a DI container
    /// </summary>
    public class Resolver
    {
        private static Resolver __resolverInstance;
        private readonly IKernel __kernel;

        private Resolver()
        {
            // build needed modules
            var modules = new INinjectModule[] { new ContainerModule() };

            // build core
            __kernel = new StandardKernel(modules);
        }

        /// <summary>
        /// Creates and initializes the DI container
        /// </summary>
        public static void Create() => __resolverInstance = __resolverInstance ?? new Resolver();
        /// <summary>
        /// Gets global Kernel variable
        /// </summary>
        // ReSharper disable once
        public static IKernel Kernel => __resolverInstance.__kernel;
    }
}