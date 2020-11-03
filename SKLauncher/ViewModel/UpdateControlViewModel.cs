using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Launcher.Core;
using Launcher.Core.Data.Updates;
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

        public UpdateControlViewModel(
            IUpdateService updateService,
            IEventService eventService)
        {
            _updateService = updateService;
            _eventService = eventService;

            // assign resolvers and subscribe to events
            _updateService.CancelDownloadResolver = _downloadCancelResolver;
            _updateService.UpdateAvailableResolver = _updateAvailableResolver;

            _updateService.Error += _updateServiceErrorHandler;
            _updateService.ReportProgress += _updateServiceReportHandler;
            _updateService.BeginDownload += _updateServiceBeginDownloadHandler;
            _updateService.EndDownload += _updateServiceEndDownloadHandler;

            // assign loc text
            Text = "Update downloading";
        }

        #region Updates handlers

        private Task<bool> _downloadCancelResolver()
        {
            var dlgResult = MessageBox.Show("Are you sure you want to stop downloading the update?",
                "Are you sure ?", MessageBoxButton.YesNo);
            var isCanceled = dlgResult == MessageBoxResult.Yes;

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
            => _eventService.ErrorEvent("Update service", e.Message);

        private void _updateServiceReportHandler(object sender, UpdateProgressEventArgs e) => Dispatcher.Invoke(() =>
        {
            Progress = e.Progress;
        });

        private void _updateServiceEndDownloadHandler(object sender, UpdateDownloadEventArgs e) => Dispatcher.Invoke(() =>
        {
            Visibility = Visibility.Collapsed;
        });

        private void _updateServiceBeginDownloadHandler(object sender, EventArgs e) => Dispatcher.Invoke(() =>
        {
            Visibility = Visibility.Visible;
        });

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
        });

        public override ICommand LoadedCommand => throw new System.NotImplementedException();

        public override ICommand UnloadedCommand => throw new System.NotImplementedException();

        #endregion
    }
}