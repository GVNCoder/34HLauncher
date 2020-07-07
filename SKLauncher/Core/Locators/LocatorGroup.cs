using Ninject;

namespace Launcher.Core.Locators
{
    public class LocatorGroup : ILocatorGroup
    {
        public LocatorGroup(IKernel kernel)
        {
            ViewModelLocator = new ViewModelLocator(kernel);
            UserControlViewModelLocator = new UserControlViewModelLocator(kernel);
        }

        public IViewModelLocator ViewModelLocator { get; }
        public IUserControlViewModelLocator UserControlViewModelLocator { get; }
    }
}