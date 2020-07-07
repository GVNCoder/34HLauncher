using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Launcher.XamlThemes.Controls.Shared;

namespace Launcher.XamlThemes.Controls
{
    [TemplatePart(Name = PartOverlay, Type = typeof(Rectangle))]
    [TemplatePart(Name = PartContainer, Type = typeof(Border))]
    public class OverlayControl : ContentControl, IOverlayControl
    {
        private const string PartOverlay = "PART_Overlay";
        private const string PartContainer = "PART_Container";

        private Rectangle _partOverlay;
        private Border _partContainer;

        private DoubleAnimation _overlayInAnimation;
        private DoubleAnimation _overlayOutAnimation;
        private DoubleAnimation _containerInAnimation;
        private DoubleAnimation _containerOutAnimation;
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _partOverlay = (Rectangle) GetTemplateChild(PartOverlay);
            _partContainer = (Border) GetTemplateChild(PartContainer);

            _partOverlay.Opacity = .0d;
            _partContainer.Opacity = .0d;

            Visibility = Visibility.Collapsed;

            _overlayInAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(150d)),
                To = 1d
            };
            _overlayOutAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(100d)),
                To = .0d
            };
            _containerInAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(70d)),
                To = 1d
            };
            _containerOutAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(50d)),
                To = .0d
            };

            Timeline.SetDesiredFrameRate(_overlayInAnimation, 30);
            Timeline.SetDesiredFrameRate(_overlayOutAnimation, 30);
            Timeline.SetDesiredFrameRate(_containerInAnimation, 30);
            Timeline.SetDesiredFrameRate(_containerOutAnimation, 30);

            _overlayInAnimation.Completed += _overlayInAnimationOnCompleted;
            _overlayOutAnimation.Completed += _overlayOutAnimationOnCompleted;

            _containerInAnimation.Completed += _containerInAnimationOnCompleted;
            _containerOutAnimation.Completed += _containerOutAnimationOnCompleted;
        }

        private void _overlayInAnimationOnCompleted(object sender, EventArgs e)
        {
            Showed?.Invoke(this, EventArgs.Empty);
        }

        private void _overlayOutAnimationOnCompleted(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void _containerInAnimationOnCompleted(object sender, EventArgs e)
        {
        }

        private void _containerOutAnimationOnCompleted(object sender, EventArgs e)
        {
        }

        private void _BeginShowAnimation()
        {
            _partOverlay.BeginAnimation(Shape.OpacityProperty, _overlayInAnimation);
            _partContainer.BeginAnimation(FrameworkElement.OpacityProperty, _containerInAnimation);
        }

        private void _BeginHideAnimation()
        {
            _partOverlay.BeginAnimation(Shape.OpacityProperty, _overlayOutAnimation);
            _partContainer.BeginAnimation(FrameworkElement.OpacityProperty, _containerOutAnimation);
        }

        #region IOverlayControl

        public event EventHandler Closed;
        public event EventHandler Showed;

        public void Show(object content)
        {
            Dispatcher.Invoke(() =>
            {
                Content = content;
                Visibility = Visibility.Visible;

                _BeginShowAnimation();
            }, DispatcherPriority.Background);
        }

        public void Hide()
        {
            Dispatcher.Invoke(_BeginHideAnimation, DispatcherPriority.Background);
        }

        #endregion
    }
}