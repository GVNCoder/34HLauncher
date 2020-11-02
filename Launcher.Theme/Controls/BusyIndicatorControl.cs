using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Launcher.XamlThemes.Controls
{
    public class BusyIndicatorControl : Control
    {
        private const string PartOverlayName = "PART_Overlay";
        private const string PartContainerName = "PART_Container";
        private const string PartAnimationEllipse = "PART_AnimationEllipse";

        private const int _FrameRate = 30;
        private const double _EllipseFullRadiusPixels = 100d;

        private Rectangle _overlayRectangle;
        private Grid _container;
        private Ellipse _animationEllipse;

        private DoubleAnimation _iOverlayAnimation;
        private DoubleAnimation _oOverlayAnimation;

        private DoubleAnimation _busyOpacityInfiniteAnimation;
        private DoubleAnimation _busyHeightInfiniteAnimation;
        private DoubleAnimation _busyWidthInfiniteAnimation;

        #region Bindable properties

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(BusyIndicatorControl), new PropertyMetadata(false, _IsOpenPropertyChangedCallback));

        // acts as a trigger, controlling animation when a property changes
        private static void _IsOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // extract arguments
            var control = (BusyIndicatorControl) d;
            var propertyValue = (bool) e.NewValue;

            // begin animation
            if (propertyValue) // in animation
            {
                control.Visibility = Visibility.Visible;
                control._container.Visibility = Visibility.Visible;

                control._overlayRectangle.BeginAnimation(FrameworkElement.OpacityProperty, control._iOverlayAnimation);

                control._animationEllipse.BeginAnimation(Shape.WidthProperty, control._busyWidthInfiniteAnimation);
                control._animationEllipse.BeginAnimation(Shape.HeightProperty, control._busyHeightInfiniteAnimation);
                control._animationEllipse.BeginAnimation(Shape.OpacityProperty, control._busyOpacityInfiniteAnimation);
            }
            else // out animation
            {
                control._container.Visibility = Visibility.Collapsed;

                control._overlayRectangle.BeginAnimation(FrameworkElement.OpacityProperty, control._oOverlayAnimation);

                control._animationEllipse.BeginAnimation(Shape.WidthProperty, null);
                control._animationEllipse.BeginAnimation(Shape.HeightProperty, null);
                control._animationEllipse.BeginAnimation(Shape.OpacityProperty, null);
            }
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(BusyIndicatorControl), new PropertyMetadata(string.Empty));

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // get template parts
            _overlayRectangle = (Rectangle) GetTemplateChild(PartOverlayName);
            _container = (Grid) GetTemplateChild(PartContainerName);
            _animationEllipse = (Ellipse) GetTemplateChild(PartAnimationEllipse);

            // setup initial state
            Visibility = Visibility.Collapsed;

            _overlayRectangle.Opacity = .0;

            // build animation
            _iOverlayAnimation = new DoubleAnimation { Duration = new Duration(TimeSpan.FromMilliseconds(150d)), To = 1d };
            _oOverlayAnimation = new DoubleAnimation { Duration = new Duration(TimeSpan.FromMilliseconds(100d)), To = .0d };

            _busyHeightInfiniteAnimation = new DoubleAnimation { RepeatBehavior = RepeatBehavior.Forever, To = _EllipseFullRadiusPixels };
            _busyWidthInfiniteAnimation = new DoubleAnimation { RepeatBehavior = RepeatBehavior.Forever, To = _EllipseFullRadiusPixels };
            _busyOpacityInfiniteAnimation = new DoubleAnimation { RepeatBehavior = RepeatBehavior.Forever, To = 0d, From = 1d };

            Timeline.SetDesiredFrameRate(_iOverlayAnimation, _FrameRate);
            Timeline.SetDesiredFrameRate(_oOverlayAnimation, _FrameRate);

            Timeline.SetDesiredFrameRate(_busyHeightInfiniteAnimation, _FrameRate);
            Timeline.SetDesiredFrameRate(_busyWidthInfiniteAnimation, _FrameRate);
            Timeline.SetDesiredFrameRate(_busyOpacityInfiniteAnimation, _FrameRate);

            // hack
            _oOverlayAnimation.Completed += (sender, e) => Visibility = Visibility.Collapsed;
        }
    }
}