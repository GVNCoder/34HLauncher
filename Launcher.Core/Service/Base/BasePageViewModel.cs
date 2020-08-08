using System.Windows.Controls;
using System.Windows.Input;

namespace Launcher.Core.Service.Base
{
    /// <summary>
    /// Defines the base class for view models for pages.
    /// </summary>
    public abstract class BasePageViewModel : BaseViewModel
    {
        protected bool _isLoaded = false;
        
        /// <summary>
        /// Gets <see cref="Page.Loaded"/> command handler
        /// </summary>
        public abstract ICommand LoadedCommand { get; }
        /// <summary>
        /// Gets <see cref="Page.Unloaded"/> command handler
        /// </summary>
        public abstract ICommand UnloadedCommand { get; }
    }
}