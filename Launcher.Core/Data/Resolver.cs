using System;

using Launcher.Core.Service;
using Launcher.Core.Service.Internal.Module;

using Ninject;
using Ninject.Modules;

namespace Launcher.Core.Data
{
    /// <inheritdoc />
    public class Resolver : IResolver
    {
        private readonly IKernel _core;

        public Resolver()
        {
            // build needed modules
            var modules = new INinjectModule[] { new ServiceModule() };

            // build core
            _core = new StandardKernel(modules);
        }

        #region Static members

        // https://csharpindepth.com/articles/singleton
        static Resolver() { }

        private static readonly Lazy<IResolver> _lazyInstance = new Lazy<IResolver>(() => new Resolver(), true);

        public static IResolver GetInstance => _lazyInstance.Value;

        #endregion

        #region IResolver

        /// <inheritdoc />
        // ReSharper disable once ConvertToAutoProperty
        public IKernel Core => _core;

        #endregion
    }
}