using System;
using System.Windows;
using System.Windows.Input;
using Launcher.Core.Interaction;

namespace Launcher.Core.Shared
{
    public class GameControlViewModel : DependencyObject, IGameControl
    {
        #region Dependency properties

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(GameControlViewModel), new PropertyMetadata("Default text"));

        public string ToolTipContent
        {
            get => (string)GetValue(ToolTipContentProperty);
            set => SetValue(ToolTipContentProperty, value);
        }
        public static readonly DependencyProperty ToolTipContentProperty =
            DependencyProperty.Register("ToolTipContent", typeof(string), typeof(GameControlViewModel), new PropertyMetadata(null));

        public bool InLoadingState
        {
            get => (bool)GetValue(InLoadingStateProperty);
            set => SetValue(InLoadingStateProperty, value);
        }
        public static readonly DependencyProperty InLoadingStateProperty =
            DependencyProperty.Register("InLoadingState", typeof(bool), typeof(GameControlViewModel), new PropertyMetadata(true));

        public Visibility GameControlVisibility
        {
            get => (Visibility)GetValue(GameControlVisibilityProperty);
            set => SetValue(GameControlVisibilityProperty, value);
        }
        public static readonly DependencyProperty GameControlVisibilityProperty =
            DependencyProperty.Register("GameControlVisibility", typeof(Visibility), typeof(GameControlViewModel), new PropertyMetadata(Visibility.Collapsed));

        public bool CanClose
        {
            get => (bool)GetValue(CanCloseProperty);
            set => SetValue(CanCloseProperty, value);
        }
        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(GameControlViewModel), new PropertyMetadata(false));

        #endregion

        #region Commands

        public ICommand CloseCommand => new DelegateCommand(obj =>
        {
            CanClose = false;
            CloseClick?.Invoke(this, EventArgs.Empty);
        });

        #endregion

        #region IGameControl

        public event EventHandler CloseClick;

        public void Show()
        {
            Dispatcher.Invoke(() =>
            {
                if (GameControlVisibility == Visibility.Visible) return;
                GameControlVisibility = Visibility.Visible;
            });
        }

        public void SetText(string text)
        {
            Dispatcher.Invoke(() => Text = text);
        }

        public void SetToolTipText(string text)
        {
            Dispatcher.Invoke(() => ToolTipContent = text);
        }

        public void SetState(bool isLoading)
        {
            Dispatcher.Invoke(() => InLoadingState = isLoading);
        }

        public void SetCanClose(bool canClose)
        {
            Dispatcher.Invoke(() => { CanClose = canClose; });
        }

        public void Hide()
        {
            Dispatcher.Invoke(() =>
            {
                if (GameControlVisibility == Visibility.Collapsed) return;
                GameControlVisibility = Visibility.Collapsed;
            });
        }

        #endregion
    }
}