using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Launcher.Core.Service;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Shared;
using Launcher.ViewModel;

namespace Launcher.Core.Data.EventLog
{
    public class EventLogService : IEventLogService
    {
        private readonly ObservableCollection<EventViewModel> _events;
        private readonly Dispatcher _dispatcher;

        public EventLogService(IViewModelSource viewModelLocator)
        {
            var application = Application.Current as App;
            _events = viewModelLocator.GetExisting<EventLogViewModel>()
                .Events;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public EventViewModel Log(EventLogLevel level, string header, string content)
        {
            var viewModel = _dispatcher.Invoke(() =>
            {
                var vm = new EventViewModel(header, (string) content.Clone(), level, _events);
                _events.Add(vm);

                return vm;
            });
            return viewModel;
        }

        public bool HasEvents => _events.Count != 0;
    }
}