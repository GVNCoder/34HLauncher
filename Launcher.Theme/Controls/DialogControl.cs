using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Launcher.XamlThemes.Controls
{
    public class DialogControl : ContentControl
    {
        private const string PartOverlayName = "PART_Overlay";
        private const string PartContainerName = "PART_ContentContainer";

        private const int _FrameRate = 30;

        private Rectangle _overlayRectangle;
        private ContentPresenter _contentContainer;

        private DoubleAnimation _iOverlayAnimation;
        private DoubleAnimation _oOverlayAnimation;
        private DoubleAnimation _iContentContainer;
        private DoubleAnimation _oContentContainer;

        #region Bindable properties

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DialogControl), new PropertyMetadata(false, _IsOpenPropertyChangedCallback));

        // acts as a trigger, controlling animation when a property changes
        private static void _IsOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // extract arguments
            var control = (DialogControl) d;
            var propertyValue = (bool) e.NewValue;

            // begin animation
            if (propertyValue) // in animation
            {
                control.Visibility = Visibility.Visible;

                control._overlayRectangle.BeginAnimation(FrameworkElement.OpacityProperty, control._iOverlayAnimation);
                control._contentContainer.BeginAnimation(FrameworkElement.OpacityProperty, control._iContentContainer);
            }
            else // out animation
            {
                control._overlayRectangle.BeginAnimation(FrameworkElement.OpacityProperty, control._oOverlayAnimation);
                control._contentContainer.BeginAnimation(FrameworkElement.OpacityProperty, control._oContentContainer);
            }
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // get template parts
            _overlayRectangle = (Rectangle) GetTemplateChild(PartOverlayName);
            _contentContainer = (ContentPresenter) GetTemplateChild(PartContainerName);

            // setup initial state
            Visibility = Visibility.Collapsed;

            _overlayRectangle.Opacity = .0;
            _contentContainer.Opacity = .0;

            // build animation
            _iOverlayAnimation = new DoubleAnimation { Duration = new Duration(TimeSpan.FromMilliseconds(150d)), To = 1d };
            _oOverlayAnimation = new DoubleAnimation { Duration = new Duration(TimeSpan.FromMilliseconds(100d)), To = .0d };
            _iContentContainer = new DoubleAnimation { Duration = new Duration(TimeSpan.FromMilliseconds(70d)), To = 1d };
            _oContentContainer = new DoubleAnimation { Duration = new Duration(TimeSpan.FromMilliseconds(50d)), To = .0d };

            Timeline.SetDesiredFrameRate(_iOverlayAnimation, _FrameRate);
            Timeline.SetDesiredFrameRate(_oOverlayAnimation, _FrameRate);
            Timeline.SetDesiredFrameRate(_iContentContainer, _FrameRate);
            Timeline.SetDesiredFrameRate(_oContentContainer, _FrameRate);

            _oOverlayAnimation.Completed += (sender, e) =>
            {
                Visibility = Visibility.Collapsed;
                Content = null;
            };
        }
    }
}