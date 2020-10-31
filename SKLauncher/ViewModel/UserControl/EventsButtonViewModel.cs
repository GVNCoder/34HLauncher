using System;
using System.Windows;
using System.Windows.Input;

using Launcher.Core;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;

namespace Launcher.ViewModel.UserControl
{
    public class EventsButtonViewModel : BaseControlViewModel
    {
        #region Base

        public override ICommand LoadedCommand => throw new NotImplementedException();
        public override ICommand UnloadedCommand => throw new NotImplementedException();

        #endregion

        #region Bindable properties

        public bool HasSomethingNew
        {
            get => (bool)GetValue(HasSomethingNewProperty);
            set => SetValue(HasSomethingNewProperty, value);
        }
        public static readonly DependencyProperty HasSomethingNewProperty =
            DependencyProperty.Register("HasSomethingNew", typeof(bool), typeof(EventsButtonViewModel), new PropertyMetadata(false));

        #endregion

        #region Commands

        public ICommand OpenEventsViewCommand => new DelegateCommand((obj) =>
        {
            // reset something new indicator
            HasSomethingNew = false;

            _navigator.Navigate("View/EventLogView.xaml");
        });

        #endregion

        private readonly IPageNavigator _navigator;

        public EventsButtonViewModel(IPageNavigator navigator, IEventService eventService)
        {
            _navigator = navigator;

            // track something new in launcher events :)
            eventService.EventOccured += _OnEventOccured;
        }

        #region Private helpers

        private void _OnEventOccured(object sender, EventOccuredEventArgs e)
        {
            HasSomethingNew = true;
        }

        #endregion
    }
}