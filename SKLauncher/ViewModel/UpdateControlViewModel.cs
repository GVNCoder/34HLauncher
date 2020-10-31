using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Launcher.Core;
using Launcher.Core.Data.Updates;
using Launcher.Core.Dialog;
using Launcher.Core.Interaction;
using Launcher.Core.Service.Base;
using Launcher.Core.Services.Updates;
using Launcher.Core.Shared;

namespace Launcher.ViewModel
{
    public class UpdateControlViewModel : BaseControlViewModel
    {
        private readonly IUpdateService _updateService;
        private readonly IEventService _eventService;
        private readonly IDialogService _dialogService;

        public UpdateControlViewModel(
            IUpdateService updateService,
            IEventService eventService,
            IDialogService dialogService)
        {
            _updateService = updateService;
            _eventService = eventService;
            _dialogService = dialogService;

            // assign resolvers and subscribe to events
            _updateService.CancelDownloadResolver = _downloadCancelResolver;
            _updateService.UpdateAvailableResolver = _updateAvailableResolver;
            _updateService.Error += _updateServiceErrorHandler;

            // assign loc text
            Text = "Update downloading";
        }

        #region Updates handlers

        private Task<bool> _downloadCancelResolver()
        {
            var dlgResult = MessageBox.Show("Are you sure you want to stop downloading the update?",
                "Are you sure ?", MessageBoxButton.YesNo);
            var isCanceled = dlgResult == MessageBoxResult.Yes;

            if (! isCanceled) Dispatcher.Invoke(() => Visibility = Visibility.Visible);

            return Task.FromResult(isCanceled);
        }

        private bool _updateAvailableResolver(LauncherVersion ver)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var dlgResult = MessageBox.Show(Application.Current.MainWindow, "Download and install it?", $"A new version is available {ver}",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            return dlgResult == MessageBoxResult.Yes;
        }

        private void _updateServiceErrorHandler(object sender, UpdateErrorEventArgs e)
        {

        }/*_eventService.Log(EventLogLevel.Error, "Update service", e.Message);*/

        #endregion

        #region Dependency props

        public Visibility Visibility
        {
            get => (Visibility)GetValue(VisibilityProperty);
            set => SetValue(VisibilityProperty, value);
        }
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(UpdateControlViewModel), new PropertyMetadata(Visibility.Collapsed));

        public int Progress
        {
            get => (int)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(int), typeof(UpdateControlViewModel), new PropertyMetadata(0));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(UpdateControlViewModel), new PropertyMetadata(string.Empty));

        #endregion

        #region Commands

        public ICommand CancelDownloadCommand => new DelegateCommand(obj =>
        {
            // cancel download process
            _updateService.CancelDownload();

            // hide control in UI
            Dispatcher.Invoke(() => Visibility = Visibility.Collapsed);
        });

        public override ICommand LoadedCommand => throw new System.NotImplementedException();

        public override ICommand UnloadedCommand => throw new System.NotImplementedException();

        #endregion
    }
}