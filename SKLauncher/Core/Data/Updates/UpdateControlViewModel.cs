using System;
using System.Windows;
using System.Windows.Input;
using Launcher.Core.Interaction;
using Launcher.Core.Services.Updates;
using Launcher.Helpers;

namespace Launcher.Core.Data.Updates
{
    public class UpdateControlViewModel : DependencyObject, IUpdateControl
    {
        #region Dependency props

        public Visibility ControlVisibility
        {
            get => (Visibility)GetValue(ControlVisibilityProperty);
            set => SetValue(ControlVisibilityProperty, value);
        }
        public static readonly DependencyProperty ControlVisibilityProperty =
            DependencyProperty.Register("ControlVisibility", typeof(Visibility), typeof(UpdateControlViewModel), new PropertyMetadata(Visibility.Collapsed));

        public int Progress
        {
            get => (int)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(int), typeof(UpdateControlViewModel), new PropertyMetadata(0));

        #endregion

        #region IUpdateControl

        public event EventHandler CancelRequested;

        private void _OnCancelRequested() => CancelRequested?.Invoke(this, EventArgs.Empty);

        public void ReportProgress(int value)
        {
            Dispatcher.Invoke(() =>
            {
                if (value != Progress)
                {
                    Progress = value;
                }
            });
        }

        public void Show()
        {
            Dispatcher.Invoke(() =>
            {
                if (ControlVisibility == Visibility.Visible) return;
                ControlVisibility = Visibility.Visible;
            });
        }

        public void Hide()
        {
            Dispatcher.Invoke(() =>
            {
                if (ControlVisibility == Visibility.Collapsed) return;
                ControlVisibility = Visibility.Collapsed;
            });
        }

        #endregion

        #region Commands

        public ICommand CancelDownloadCommand => new DelegateCommand(_CancelDownloadExec);

        private void _CancelDownloadExec(object obj)
        {
            _OnCancelRequested();
        }

        #endregion
    }
}