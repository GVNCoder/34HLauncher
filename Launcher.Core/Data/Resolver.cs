using System;

using Launcher.Core.Service.Internal.Module;

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

        public Resolver()
        {
            // build needed modules
            var modules = new INinjectModule[] { new ServiceModule() };

            // build core
            var core = new StandardKernel(modules);
        }

        /// <summary>
        /// Creates and initializes the DI container
        /// </summary>
        public static void Create() => __resolverInstance = __resolverInstance ?? new Resolver();
    }
}