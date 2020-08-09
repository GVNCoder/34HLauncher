using System.Windows.Controls;
using System.Windows.Input;

namespace Launcher.Core.Service.Base
{
    /// <summary>
    /// Defines the base class for view models for controls.
    /// </summary>
    public abstract class BaseControlViewModel : BaseViewModel
    {
        protected bool _isLoaded = false;
        
        /// <summary>
        /// Gets <see cref="Control.Loaded"/> command handler
        /// </summary>
        public abstract ICommand LoadedCommand { get; }
        /// <summary>
        /// Gets <see cref="Control.Unloaded"/> command handler
        /// </summary>
        public abstract ICommand UnloadedCommand { get; }
    }
}