using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

using Launcher.Core.Interaction;

namespace Launcher.Core.Services
{
    public class WindowContentNavigationService : IWindowContentNavigationService
    {
        private readonly Dispatcher _dispatcher;
        private NavigationService _navigationService;

        public WindowContentNavigationService()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            Navigate = new DelegateCommand(obj => NavigateTo(obj as string));
            Back = new DelegateCommand(obj => GoBack());
            Forward = new DelegateCommand(obj => GoForward());
        }

        public void GoBack()
        {
            _dispatcher.Invoke(() =>
            {
                if (_navigationService.CanGoBack)
                    _navigationService.GoBack();
            });
        }

        public void GoForward()
        {
            _dispatcher.Invoke(() =>
            {
                if (_navigationService.CanGoForward)
                    _navigationService.GoForward();
            });
        }

        public void NavigateTo(string target)
        {
            var source = new Uri(target, UriKind.Relative);

            _dispatcher.Invoke(() =>
            {
                var isCurrentTarget = _thisIsSamePage(source);
                if (!isCurrentTarget)
                    _navigationService.Navigate(source);
            });
        }

        public void Initialize(Frame from)
        {
            _navigationService = from.NavigationService;
            _navigationService.Navigating += _navigationInitiatedHandler;
        }

        private void _navigationInitiatedHandler(object sender, NavigatingCancelEventArgs e)
        {
            Navigation?.Invoke(this, EventArgs.Empty);
        }

        public ICommand Navigate { get; }
        public ICommand Back { get; }
        public ICommand Forward { get; }

        public event EventHandler Navigation;

        private bool _thisIsSamePage(Uri source)
        {
            if (_navigationService.Source == null) return false;

            var inputUriString = source.ToString().Replace(@"\", "/");
            var currentUriString = _navigationService.Source.ToString();

            return string.Equals(inputUriString, currentUriString);
        }
    }
}