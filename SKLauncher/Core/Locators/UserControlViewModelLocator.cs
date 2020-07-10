using System;
using Launcher.Core.Shared;
using Ninject;

namespace Launcher.Core.Locators
{
    public class UserControlViewModelLocator : IUserControlViewModelLocator
    {
        private readonly Lazy<GameControlViewModel> _gameControlViewModel;

        public UserControlViewModelLocator() { } // for xaml markup

        public UserControlViewModelLocator(IKernel kernel) // code behind
        {
            _gameControlViewModel = new Lazy<GameControlViewModel>(() => kernel.Get<GameControlViewModel>());
        }

        public GameControlViewModel GameControlViewModel => _gameControlViewModel.Value;
    }
}