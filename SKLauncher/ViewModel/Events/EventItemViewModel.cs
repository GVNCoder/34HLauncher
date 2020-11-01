using System.Windows;

using Launcher.Core;

namespace Launcher.ViewModel
{
    public class EventItemViewModel : DependencyObject
    {
        #region Bindable properties

        public string EventName
        {
            get => (string)GetValue(EventNameProperty);
            set => SetValue(EventNameProperty, value);
        }
        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register("EventName", typeof(string), typeof(EventItemViewModel), new PropertyMetadata(string.Empty));

        public string Content
        {
            get => (string)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(EventItemViewModel), new PropertyMetadata(string.Empty));

        public EventType EventType
        {
            get => (EventType)GetValue(EventTypeProperty);
            set => SetValue(EventTypeProperty, value);
        }
        public static readonly DependencyProperty EventTypeProperty =
            DependencyProperty.Register("EventType", typeof(EventType), typeof(EventItemViewModel), new PropertyMetadata(Core.EventType.Info));

        public string TimeCreated
        {
            get => (string)GetValue(TimeCreatedProperty);
            set => SetValue(TimeCreatedProperty, value);
        }
        public static readonly DependencyProperty TimeCreatedProperty =
            DependencyProperty.Register("TimeCreated", typeof(string), typeof(EventItemViewModel), new PropertyMetadata(string.Empty));

        #endregion
    }
}