using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Launcher.Core.Service
{
    /// <summary>
    /// Defines a navigator that allows you to navigate through the application pages,
    /// track their change, as well as preprocess their navigation.
    /// </summary>
    public interface IPageNavigator
    {
        /// <summary>
        /// Navigates to the specified page
        /// </summary>
        /// <param name="uri">The page uri</param>
        void Navigate(string uri);
        /// <summary>
        /// If possible, navigates through the history of transitions back
        /// </summary>
        void NavigateBack();
        /// <summary>
        /// If possible, navigates forward navigation history
        /// </summary>
        void NavigateForward();
        /// <summary>
        /// Gets the current page
        /// </summary>
        Page CurrentPage { get; }
        /// <summary>
        /// Gets the content presenter
        /// </summary>
        Frame Container { get; }
        /// <summary>
        /// Occurs when navigation was initiated
        /// </summary>
        event EventHandler<NavigatingCancelEventArgs> NavigationInitiated;
        /// <summary>
        /// Occurs when navigation has been completed
        /// </summary>
        event EventHandler<NavigationEventArgs> Navigated;
    }
}