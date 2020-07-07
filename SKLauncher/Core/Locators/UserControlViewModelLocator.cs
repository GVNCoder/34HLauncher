using Ninject;

namespace Launcher.Core.Locators
{
    public class UserControlViewModelLocator : IUserControlViewModelLocator
    {
        public UserControlViewModelLocator() { } // for xaml markup

        public UserControlViewModelLocator(IKernel kernel) // code behind
        {
            
        }
    }
}