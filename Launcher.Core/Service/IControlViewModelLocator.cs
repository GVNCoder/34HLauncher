using Launcher.Core.Service.Base;

namespace Launcher.Core.Service
{
    /// <summary>
    /// Defines a locator for control view models.
    /// </summary>
    public interface IControlViewModelLocator
    {
        // template of declaration a new viewModel in locator
        BaseControlViewModel NameViewModel { get; }
    }
}