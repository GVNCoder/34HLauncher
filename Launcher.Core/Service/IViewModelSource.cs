using System;
using Launcher.Core.Service.Base;

namespace Launcher.Core.Service
{
    /// <summary>
    /// Defines a view model source
    /// </summary>
    public interface IViewModelSource
    {
        /// <summary>
        /// Creates a new ViewModel instance
        /// </summary>
        /// <typeparam name="TViewModel">View model type</typeparam>
        /// <returns>Created view model instance</returns>
        TViewModel Create<TViewModel>() where TViewModel : BaseViewModel;
        /// <summary>
        /// Gets existing ViewModel instance
        /// </summary>
        /// <typeparam name="TViewModel">View model type</typeparam>
        /// <exception cref="InvalidOperationException">Occurs, when requested ViewModel type not found</exception>
        /// <returns>Found ViewModel instance</returns>
        TViewModel GetExisting<TViewModel>() where TViewModel : BaseViewModel;
    }
}