namespace Launcher.Core.Locators
{
    public interface ILocatorGroup
    {
        IViewModelLocator ViewModelLocator { get; }
        IUserControlViewModelLocator UserControlViewModelLocator { get; }
    }
}