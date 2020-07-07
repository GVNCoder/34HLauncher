using System.Windows;
using System.Windows.Controls;

namespace Launcher.XamlThemes.Controls
{
    public class EventsButton : Button
    {
        static EventsButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EventsButton),
                new FrameworkPropertyMetadata(typeof(EventsButton)));
        }

        public bool HasEvents
        {
            get => (bool)GetValue(HasEventsProperty);
            set => SetValue(HasEventsProperty, value);
        }
        public static readonly DependencyProperty HasEventsProperty =
            DependencyProperty.Register("HasEvents", typeof(bool), typeof(EventsButton), new PropertyMetadata(false));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EventsButton), new PropertyMetadata(string.Empty));
    }
}