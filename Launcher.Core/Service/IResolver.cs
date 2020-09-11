using Ninject;

namespace Launcher.Core.Service
{
    /// <summary>
    /// Defines the center for resolving dependencies
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// Gets access to the DI container
        /// </summary>
        IKernel Core { get; }
    }
}