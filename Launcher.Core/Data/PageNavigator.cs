using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

using Launcher.Core.Helper;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;

namespace Launcher.Core.Data
{
    public class PageNavigator : IPageNavigator, IUIHostDependency
    {
        #region Private fields

        private const string __HOST_NAME = "NavigationService";
        
        private readonly Dispatcher _dispatcher;
        private NavigationService _navigationService;
        private Uri _currentUri;

        #endregion

        public PageNavigator()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }
        
        #region IPageNavigator

        public void Navigate(string uri)
        {
            _dispatcher.Invoke(() => _navigationService?.Navigate(new Uri(uri, UriKind.Relative)));
        }

        public void NavigateBack()
        {
            _dispatcher.Invoke(() =>
            {
                if (_navigationService.CanGoBack)
                    _navigationService.GoBack();
            });
        }

        public void NavigateForward()
        {
            _dispatcher.Invoke(() =>
            {
                if (_navigationService.CanGoForward)
                    _navigationService.GoForward();
            });
        }

        public Page CurrentPage => (Page) _navigationService?.Content;
        public Frame Container { get; private set; }
        
        public event EventHandler<NavigatingCancelEventArgs> NavigationInitiated;
        public event EventHandler<NavigationEventArgs> Navigated;

        #endregion

        #region IUIDependency

        public void SetDependency(FrameworkElement element)
        {
            Container = (Frame) element;

            // extract NavigationService instance
            _navigationService = ReflectionHelper.GetPropertyInstance<NavigationService>(element, __HOST_NAME);
            
            // subscribe to events
            _navigationService.Navigating += _HandleOnNavigatingEvent;
            _navigationService.Navigated += _HandleOnNavigatedEvent;
        }

        #endregion

        #region Private helpers

        private static bool _IsSamePage(Uri current, Uri target) => current == target;

        private void _HandleOnNavigatingEvent(object sender, NavigatingCancelEventArgs e)
        {
            if (_IsSamePage(_currentUri, e.Uri)) e.Cancel = true;
            else _OnNavigationInitiated(e); // pass event
        }

        private void _HandleOnNavigatedEvent(object sender, NavigationEventArgs e)
        {
            _currentUri = e.Uri;
            _OnNavigated(e);
        }
        
        private void _OnNavigationInitiated(NavigatingCancelEventArgs e)
            => NavigationInitiated?.Invoke(this, e);
        
        private void _OnNavigated(NavigationEventArgs e)
            => Navigated?.Invoke(this, e);

        #endregion
    }
}