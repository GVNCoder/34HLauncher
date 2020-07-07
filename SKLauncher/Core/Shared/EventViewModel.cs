using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Launcher.Core.Interaction;
using Clipboard = Launcher.Helpers.Clipboard;

namespace Launcher.Core.Shared
{
    public class EventViewModel : DependencyObject
    {
        private readonly IList<EventViewModel> _events;

        public EventViewModel(string header, string content, EventLogLevel level, IList<EventViewModel> events)
        {
            Header = header;
            Content = content;
            Level = level;

            _events = events;
        }

        public ICommand CloseCommand => new DelegateCommand(obj => _events.Remove(this));

        public ICommand CopyCommand => new DelegateCommand(obj =>
        {
            var eventViewModel = _events[_events.IndexOf(this)];
            var toCopy = $"{eventViewModel.Header}\n{eventViewModel.Content}";
            Clipboard.CopyToClipboard(toCopy);
        });

        public string Header { get; }
        public EventLogLevel Level { get; }

        public string Content
        {
            get => (string)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(EventViewModel), new PropertyMetadata(string.Empty));
    }
}