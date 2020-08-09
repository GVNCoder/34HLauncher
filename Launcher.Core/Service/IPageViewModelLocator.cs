using Launcher.Core.Service.Base;

namespace Launcher.Core.Service
{
    /// <summary>
    /// Defines a locator for page view models.
    /// </summary>
    public interface IPageViewModelLocator
    {
        // template of declaration a new viewModel in locator
        BasePageViewModel NameViewModel { get; }
    }
}